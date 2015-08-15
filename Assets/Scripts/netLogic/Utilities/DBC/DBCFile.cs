using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using netLogic.Utilities.MPQ;

namespace netLogic.Utilities.DBC
{
    public interface IDBCRowConverter<T>
        where T : new()
    {
        T Convert(object value);
    }

    public class DBCFile<T> where T : new()
    {
        public DBCFile(string mpqName)
        {
            mFile = new MPQFile(mpqName);
            mReader = new System.IO.BinaryReader(mFile);
            FileName = mpqName;
            mCreationType = typeof(T);
        }

        public bool LoadData()
        {
            byte[] signature = mReader.ReadBytes(4);
            string str = Encoding.UTF8.GetString(signature);
            if (str != "WDBC")
                throw new Exception("Invalid signature in DBC file!");

            Type type = mCreationType;
            var fields = type.GetFields();
            int fieldCount = 0;
            foreach (var field in fields)
            {
                switch (Type.GetTypeCode(field.FieldType))
                {
                    case TypeCode.String:
                        {
                        var attribs = field.GetCustomAttributes(typeof(LocalizedAttribute), false);
                        if (attribs == null || attribs.Length == 0)
                            ++fieldCount;
                        else
                            fieldCount += 17;
                        }
                        break;

                    case TypeCode.Object:
                        {
                            if (field.FieldType.IsArray)
                            {
                                var attribs = field.GetCustomAttributes(typeof(ArrayAttribute), false);
                                if (attribs == null || attribs.Length == 0)
                                    throw new InvalidOperationException("For arrays the [Array] attribute must set with the desired size of the array!");

                                fieldCount += (int)(attribs[0] as ArrayAttribute).Length;
                            }
                        }
                        break;

                    default:
                        ++fieldCount;
                        break;
                }
            }

            uint numRecords = mReader.ReadUInt32();
            uint numFields = mReader.ReadUInt32();
            uint recordSize = mReader.ReadUInt32();
            uint stringSize = mReader.ReadUInt32();

            mReader.BaseStream.Position = numRecords * recordSize + 20;
            byte[] stringData = mReader.ReadBytes((int)stringSize);
            string fullStr = Encoding.UTF8.GetString(stringData);
            string[] strings = fullStr.Split(new string[] { "\0" }, StringSplitOptions.None);
            Dictionary<int, string> stringTable = new Dictionary<int, string>();
            int curPos = 0;
            foreach (var strEnt in strings)
            {
                stringTable.Add(curPos, strEnt);
                curPos += Encoding.UTF8.GetByteCount(strEnt) + 1;
            }
            mReader.BaseStream.Position = 20;

            if (numFields != fieldCount)
                throw new Exception("numFields != fieldCount in DBC file!");

            for (uint i = 0; i < numRecords; ++i)
            {
                object t = Activator.CreateInstance(mCreationType);
                long posStart = mReader.BaseStream.Position;
                foreach (var field in fields)
                {
                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        case TypeCode.Int32:
                            {
                                int value = mReader.ReadInt32();
                                field.SetValue(t, value);
                                break;
                            }

                        case TypeCode.UInt32:
                            {
                                uint uvalue = mReader.ReadUInt32();
                                field.SetValue(t, uvalue);
                                break;
                            }

                        case TypeCode.String:
                            {
                                var attribs = field.GetCustomAttributes(typeof(LocalizedAttribute), false);
                                if (attribs.Length == 0)
                                {
                                    int offset = mReader.ReadInt32();
                                    if (stringTable.ContainsKey(offset) == false)
                                        throw new InvalidOperationException("Invalid index into stringtable found!");

                                    string strVal = stringTable[offset];
                                    field.SetValue(t, strVal);
                                }
                                else
                                {
                                    string strValue = "";
                                    for (uint j = 0; j < 17; ++j)
                                    {
                                        int offset = mReader.ReadInt32();
                                        if (strValue == "" && offset != 0 && stringTable.ContainsKey(offset))
                                        {
                                            strValue = stringTable[offset];
                                            LocalePosition = j;
                                        }
                                    }

                                    field.SetValue(t, strValue);
                                }
                                break;
                            }

                        case TypeCode.Object:
                            {
                                // Info: Checks if type is array already made where numFields is calculated.
                                Type atype = field.FieldType.GetElementType();
                                var attribs = field.GetCustomAttributes(typeof(ArrayAttribute), false);
                                int len = (int)(attribs[0] as ArrayAttribute).Length;
                                Array array = Array.CreateInstance(atype, len);
                                for (int q = 0; q < len; ++q)
                                {
                                    switch (Type.GetTypeCode(atype))
                                    {
                                        case TypeCode.Int32:
                                            array.SetValue(mReader.ReadInt32(), q);
                                            break;

                                        case TypeCode.UInt32:
                                            array.SetValue(mReader.ReadUInt32(), q);
                                            break;

                                        case TypeCode.Single:
                                            array.SetValue(mReader.ReadSingle(), q);
                                            break;
                                    }
                                }

                                field.SetValue(t, array);
                            }
                            break;

                        case TypeCode.Single:
                            {
                                float fvalue = mReader.ReadSingle();
                                field.SetValue(t, fvalue);
                                break;
                            }
                    }
                }

                long posEnd = mReader.BaseStream.Position;
                long diff = posEnd - posStart;
                var firstVal = fields[0].GetValue(t);
                uint id = (uint)Convert.ChangeType(firstVal, typeof(uint));
                if (mConverter == null)
                    mRecords.Add(id, (T)t);
                else
                    mRecords.Add(id, mConverter.Convert(t));
            }

            return true;
        }

        private Dictionary<uint, T> mRecords = new Dictionary<uint, T>();
        private MPQFile mFile;
        private System.IO.BinaryReader mReader;
        private Type mCreationType;
        private IDBCRowConverter<T> mConverter = null;

        public void SetLoadType(Type load, IDBCRowConverter<T> converter)
        {
            mCreationType = load;
            mConverter = converter;
        }

        public void AddEntry(uint id, T value)
        {
            if (mRecords.ContainsKey(id))
                throw new Exception("Key already exists!");

            mRecords.Add(id, value);
        }

        public T this[uint id]
        {
            get { return mRecords[id]; }
        }

        public bool ContainsKey(uint id)
        {
            return mRecords.ContainsKey(id);
        }

        public uint MaxKey { get { return mRecords.Keys.Max(); } }

        public Dictionary<uint, T>.ValueCollection Records { get { return mRecords.Values; } }
        public string FileName { get; private set; }
        public uint LocalePosition { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class LocalizedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ArrayAttribute : Attribute
    {
        public ArrayAttribute(uint len)
        {
            Length = len;
        }

        public uint Length { get; private set; }
    }
}
