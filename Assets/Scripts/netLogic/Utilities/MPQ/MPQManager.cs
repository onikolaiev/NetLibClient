using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

namespace netLogic.Utilities.MPQ
{
   /* public static class ListExtensions
    {
        public static List<T> ToList<T>(this Array items, Func<object, T> mapFunction)
        {
            if (items == null || mapFunction == null)
                return new List<T>();

            List<T> coll = new List<T>();
            for (int i = 0; i < items.Length; i++)
            {
                T val = mapFunction(items.GetValue(i));
                if (val != null)
                    coll.Add(val);
            }
            return coll;
        }
        public static List<T> ToList<T>(this string[] items)
        {
            return items.ToList<T>(o => { return (T)o; });
        }
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            return new List<TSource>(source);
        } 
    }

    */

    internal class MPQArchiveLoader : Singleton<MPQArchiveLoader>
    {
        #region Imported Functions

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool SFileOpenArchive(string archiveName, uint priority, uint flags, ref IntPtr hArchive);

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool SFileOpenFileEx(IntPtr hArchive, string fileName, uint searchScope, ref IntPtr hFile);

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern uint SFileGetFileSize(IntPtr hFile, ref uint fileSizeHigh);

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool SFileReadFile(IntPtr hFile, [In, Out] byte[] lpBuffer, int numBytes, ref int bytesRead, IntPtr lpOverlapped);

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SFileCloseFile(IntPtr hArchive);

        [DllImport("Stormlib.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SFileHasFile(IntPtr hArchive, string file);

        #endregion

        public enum Locales
        {
            enGB,
            deDE,
            frFR,
            enUS,
            esES,
            esMX,
            ruRU,
            chCH,
            Unknown
        }

        public Locales Locale { get; private set; }

        public event Action Initialized;

        /*
        public async Task<bool> Init()
        {
            ArchiveList = new Dictionary<string, MPQArchive>();

            Func<bool> a = new Func<bool>(() =>
            {
                string basePath = Game.GameManager.GamePath;
                basePath += "\\data";
                LoadArchivesFromDir(basePath);

                Locale = Locales.Unknown;

                var locs = Enum.GetNames(typeof(Locales));
                foreach (var loc in locs)
                {
                    if (System.IO.Directory.Exists(basePath + "\\" + loc))
                    {
                        Locale = (Locales)Enum.Parse(typeof(Locales), loc);
                        break;
                    }
                }

                if (Locale == Locales.Unknown)
                    throw new Exception("Unable to determine locale!");

                basePath += "\\" + Locale.ToString();
                LoadArchivesFromDir(basePath);

                if (Initialized != null)
                    Initialized();

                return true;
            }
            );

            TaskFactory<bool> factory = new TaskFactory<bool>();
            return await factory.StartNew(a);
        }
        */

        private void LoadArchive(List<string> listFiles)
        {
            foreach (string file in listFiles)
            {
                IntPtr hArchive = new IntPtr(0);
                bool ret = SFileOpenArchive(file, 0, 0, ref hArchive);
                if (ret)
                {
                    Archives.Add(file, hArchive);
                    ArchiveList.Add(file, new MPQArchive(hArchive));
                }
            }
        }

        private void LoadArchivesFromDir(string dir)
        {
            string[] files = System.IO.Directory.GetFiles(dir, "*.mpq");
            List<string> listFiles = files.ToList();
            SortLambda(listFiles);
            LoadArchive(listFiles);
        }

        private void SortLambda(List<string> listFiles)
        {
            listFiles.Sort(
                (strA, strB) =>
                {
                    if (strA == null && strB == null)
                        return 0;
                    if (strA == null && strB != null)
                        return 1;
                    if (strA != null && strB == null)
                        return -1;

                    int patchIndexA = strA.ToLower().IndexOf("patch");
                    int patchIndexB = strB.ToLower().IndexOf("patch");

                    if (patchIndexA != -1 && patchIndexB == -1)
                        return -1;
                    if (patchIndexB != -1 && patchIndexA == -1)
                        return 1;

                    if (patchIndexA == -1 && patchIndexB == -1)
                        return strA.CompareTo(strB);

                    int extIndexA = strA.LastIndexOf('.');
                    int extIndexB = strB.LastIndexOf('.');

                    char patchIdentA = strA.Substring(extIndexA - 1, 1)[0];
                    char patchIdentB = strB.Substring(extIndexB - 1, 1)[0];

                    char separatorA = strA.Substring(extIndexA - 2, 1)[0];
                    char separatorB = strB.Substring(extIndexB - 2, 1)[0];

                    if (separatorA != '-' && separatorB == '-')
                        return 1;
                    if (separatorA == '-' && separatorB != '-')
                        return -1;

                    if (patchIdentA == patchIdentB)
                        return 0;
                    if (patchIdentA < patchIdentB)
                        return 1;
                    if (patchIdentB < patchIdentA)
                        return -1;

                    return 0;
                }
            );
        }

        internal Dictionary<string, IntPtr> Archives = new Dictionary<string, IntPtr>();

        public Dictionary<string, MPQArchive> ArchiveList { get; private set; }
    }

    public class MPQArchive
    {
        public MPQArchive(IntPtr handle)
        {
            mHandle = handle;
        }

        public string[] GetFiles()
        {
            IntPtr lf = IntPtr.Zero;
            bool ret = MPQArchiveLoader.SFileOpenFileEx(mHandle, "(listfile)", 0, ref lf);
            if (ret == false)
                return new string[] { };

            MPQFile file = new MPQFile();
            file.Load(lf);
            var bytes = file.Read(file.FileSize);
            string fullStr = Encoding.UTF8.GetString(bytes);
            return fullStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private IntPtr mHandle;
    }

    public class MPQFile : Stream
    {
        private byte[] fileData;
        private IntPtr fileHandle = IntPtr.Zero;

        public static bool Exists(string fileName)
        {
            foreach (var val in MPQArchiveLoader.Instance.Archives)
            {
                if (MPQArchiveLoader.SFileHasFile(val.Value, fileName))
                    return true;
            }

            return false;
        }

        public MPQFile(string fileName)
        {
            lock (mAccessLock)
            {
                FileName = fileName;
                foreach (KeyValuePair<string, IntPtr> hArchive in MPQArchiveLoader.Instance.Archives)
                {
                    bool ret = MPQArchiveLoader.SFileOpenFileEx(hArchive.Value, fileName, 0, ref fileHandle);
                    if (ret)
                        break;
                    else
                        fileHandle = IntPtr.Zero;
                }

                if (fileHandle == IntPtr.Zero)
                    throw new System.IO.FileNotFoundException("No MPQ-Archive contains the file!", fileName);

                Load(fileHandle);
            }
        }

        internal MPQFile()
        {
        }

        internal void Load(IntPtr hFile)
        {
            uint sizeHigh = 0;
            uint fileSize = MPQArchiveLoader.SFileGetFileSize(hFile, ref sizeHigh);

            if (fileSize == 0)
                throw new System.IO.FileNotFoundException("No MPQ-Archive contains the file with valid size!", FileName);

            fileData = new byte[fileSize];
            int bytesRead = 0;

            FileSize = fileSize;
            Position = 0;
            MPQArchiveLoader.SFileReadFile(hFile, fileData, (int)fileSize, ref bytesRead, IntPtr.Zero);
            MPQArchiveLoader.SFileCloseFile(hFile);
        }

        public uint FileSize { get; private set; }
        public override long Position { get; set; }
        public string FileName { get; private set; }

        public GCHandle GetPointer()
        {
            var handle = GCHandle.Alloc(fileData, GCHandleType.Pinned);
            return handle;
        }

        public void Read<T>(ref T input) where T : struct
        {
            int size = Marshal.SizeOf(input);
            if (Position + size > FileSize)
                throw new ArgumentOutOfRangeException();

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(fileData, (int)Position, ptr, size);
            input = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            Position += (uint)size;
        }

        public void Read<T>(T[] arr) where T : struct
        {
            GCHandle hdl = GCHandle.Alloc(arr, GCHandleType.Pinned);
            int size = Marshal.SizeOf(typeof(T)) * arr.Length;
            byte[] bt = Read((uint)size);
            Marshal.Copy(bt, 0, hdl.AddrOfPinnedObject(), size);
            hdl.Free();
        }

        public T ReadAt<T>(uint offset) where T : struct
        {
            var oldPos = Position;
            Position = offset;
            T ret = Read<T>();
            Position = oldPos;
            return ret;
        }

        public T Read<T>() where T : struct
        {
            T ret = new T();
            Read<T>(ref ret);
            return ret;
        }

        public byte[] Read(uint numBytes)
        {
            if (Position + numBytes > FileSize)
                throw new ArgumentOutOfRangeException();

            byte[] ret = new byte[numBytes];
            Array.Copy(fileData, (int)Position, ret, 0, (int)numBytes);
            Position += numBytes;
            return ret;
        }

        public uint ReadUInt()
        {
            if (Position + 4 > FileSize)
                throw new ArgumentOutOfRangeException();
            Position += 4;
            return BitConverter.ToUInt32(fileData, (int)Position - 4);
        }

        public short ReadShort()
        {
            if (Position + 2 > FileSize)
                throw new ArgumentOutOfRangeException();
            Position += 2;
            return BitConverter.ToInt16(fileData, (int)Position - 2);
        }

        public float ReadFloat()
        {
            if (Position + 4 > FileSize)
                throw new ArgumentOutOfRangeException();
            Position += 4;
            return BitConverter.ToSingle(fileData, (int)Position - 4);
        }

        public static MPQFile FromHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            MPQFile ret = new MPQFile();
            ret.Load(handle);
            ret.FileName = "";
            return ret;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override void Close()
        {
        }
        public override long Length
        {
            get { return FileSize; }
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;

                case SeekOrigin.Current:
                    Position += offset;
                    break;

                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }

            return Position;
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] bytes = Read((uint)count);
            Buffer.BlockCopy(bytes, 0, buffer, offset, count);
            return bytes.Length;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private static object mAccessLock = new object();
    }
}

// List of all MPQs
// "backup.MPQ", "base.MPQ", "dbc.MPQ", "expansion.MPQ", "expansionLoc.MPQ", "fonts.MPQ", "interface.MPQ", "misc.MPQ", "model.MPQ", "patch.MPQ", "sound.MPQ", "speech.MPQ", "terrain.MPQ", "texture.MPQ", "wmo.MPQ" };