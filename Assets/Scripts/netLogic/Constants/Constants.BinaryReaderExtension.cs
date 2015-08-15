using netLogic.Network;
using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace netLogic.Constants
{
    public static class BinaryReaderExtensions
    {
        /// <summary>  Reads the packed guid from the current stream and 
        /// advances the current position of the stream by packed guid size.
        /// </summary>
        public static ulong ReadPackedGuid(this BinaryReader reader)
        {
            byte mask = reader.ReadByte();

            if (mask == 0)
            {
                return 0;
            }

            ulong res = 0;

            int i = 0;
            while (i < 8)
            {
                if ((mask & 1 << i) != 0)
                {
                    res += (ulong)reader.ReadByte() << (i * 8);
                }
                i++;
            }

            return res;
        }

        /// <summary> Reads the NULL terminated string from 
        /// the current stream and advances the current position of the stream by string length + 1.
        /// <seealso cref="BinaryReader.ReadString"/>
        /// </summary>
        public static string ReadCString(this BinaryReader reader)
        {
            return reader.ReadCString(Encoding.UTF8);
        }

        /// <summary> Reads the NULL terminated string from 
        /// the current stream and advances the current position of the stream by string length + 1.
        /// <seealso cref="BinaryReader.ReadString"/>
        /// </summary>
        public static string ReadCString(this BinaryReader reader, Encoding encoding)
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return encoding.GetString(bytes.ToArray());
        }

        public static float ReadFloat(this BinaryReader reader)
        {
            
            return System.BitConverter.ToSingle(reader.ReadBytes(4), 0);
        }


        /// <summary> Reads the object coordinates from 
        /// the current stream and advances the current position of the stream by 12 bytes.
        /// </summary>
        public static Vector3 ReadCoords3(this BinaryReader reader)
        {
            Vector3 v = new Vector3();

            v.x = reader.ReadFloat();
            v.z = reader.ReadFloat();
            v.y = reader.ReadFloat();
             

            return v;
        }

        /// <summary> Reads the object coordinates and orientation from 
        /// the current stream and advances the current position of the stream by 16 bytes.
        /// </summary>
        public static Coords4 ReadCoords4(this BinaryReader reader)
        {
            Coords4 v;

            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
            v.O = reader.ReadSingle();

            return v;
        }

        /// <summary> Reads struct from the current stream and advances the 
        /// current position if the stream by SizeOf(T) bytes.
        /// </summary>
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            byte[] rawData = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            var returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return returnObject;
        }
    }
}
