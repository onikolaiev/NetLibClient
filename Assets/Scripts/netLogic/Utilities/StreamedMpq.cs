using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using netLogic.Utilities.MPQ;

namespace netLogic.Utilities
{
    public class StreamedMpq : System.IO.Stream
    {
        private List<MPQFile> mStreamFiles = new List<MPQFile>();

        public StreamedMpq(List<MPQFile> files)
        {
            mStreamFiles = files;
            long curOffset = 0;
            foreach (var file in files)
            {
                mOffsets.Add(curOffset);
                mFileSizes.Add(file.FileSize);
                curOffset += file.FileSize;
            }
        }

        public T Read<T>() where T : struct
        {
            T ret = default(T);
            Read(ref ret);
            return ret;
        }

        public void Read<T>(ref T input) where T : struct
        {
            int size = Marshal.SizeOf(input);
            uint currentFileSize = mStreamFiles[mFileIndex].FileSize;
            int bytesAvailable = (int)currentFileSize - mFileOffset;

            // If we are at the last file bytesAvailable MUST be enough to read!
            if (mFileIndex == mStreamFiles.Count - 1 && bytesAvailable < size)
                throw new ArgumentOutOfRangeException();

            int tmpFileIndex = mFileIndex;
            // Add the sizes of the remaining files until we have enough available
            while (bytesAvailable < size)
            {
                ++tmpFileIndex;
                bytesAvailable += (int)mStreamFiles[tmpFileIndex].FileSize;
                if (bytesAvailable >= size)
                    break;

                // at the last file and still not enough bytes available. I have something for you: We wont get any more bytes!
                if (tmpFileIndex == mStreamFiles.Count - 1)
                    throw new ArgumentOutOfRangeException();
            }

            byte[] fullData = new byte[size];
            int bytesRead = 0;
            var curFile = mStreamFiles[mFileIndex];
            curFile.Position = mFileOffset;
            var bRead = Math.Min(curFile.FileSize - mFileOffset, size);
            Buffer.BlockCopy(curFile.Read((uint)bRead), 0, fullData, 0, (int)bRead);
            bytesRead += (int)bRead;

            while (bytesRead < size)
            {
                ++mFileIndex;
                curFile = mStreamFiles[mFileIndex];
                bRead = Math.Min(curFile.FileSize, (size - bytesRead));
                Buffer.BlockCopy(curFile.Read((uint)bRead), 0, fullData, bytesRead, (int)bRead);
                bytesRead += (int)bRead;
            }

            mFileOffset = (int)mStreamFiles[mFileIndex].Position;
/*
            fixed (byte* ptr = fullData)
            {
                input = (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
            }*/

            Position = mOffsets[mFileIndex] + mFileOffset;
        }

        private int mFileIndex = 0;
        private int mFileOffset = 0;
        private long mFullPosition = 0;
        private List<uint> mFileSizes = new List<uint>();
        private List<long> mOffsets = new List<long>();

        private void PositionChanged(long newPos)
        {
            if (newPos < 0)
                throw new ArgumentException("newPos < 0");

            if (newPos > mOffsets.Last() + mFileSizes.Last())
                throw new ArgumentException("newPos > fullSize");

            mFullPosition = newPos;

            for (int i = 0; i < mOffsets.Count; ++i)
            {
                if (mOffsets[i] <= newPos && mOffsets[i] + mFileSizes[i] >= newPos)
                {
                    mFileIndex = i;
                    mFileOffset = (int)(newPos - mOffsets[i]);
                    return;
                }
            }

            throw new ArgumentException("newPos is invalid!");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int size = count;
            uint currentFileSize = mStreamFiles[mFileIndex].FileSize;
            int bytesAvailable = (int)currentFileSize - mFileOffset;
            if (mFileIndex == mStreamFiles.Count - 1 && bytesAvailable < size)
                throw new ArgumentOutOfRangeException();

            int tmpFileIndex = mFileIndex;
            while (bytesAvailable < size)
            {
                ++tmpFileIndex;
                bytesAvailable += (int)mStreamFiles[tmpFileIndex].FileSize;
                if (bytesAvailable >= size)
                    break;

                if (tmpFileIndex == mStreamFiles.Count - 1)
                    throw new ArgumentOutOfRangeException();
            }

            byte[] fullData = new byte[size];
            int bytesRead = 0;
            var curFile = mStreamFiles[mFileIndex];
            curFile.Position = mFileOffset;
            var bRead = Math.Min(curFile.FileSize - mFileOffset, size);
            Buffer.BlockCopy(curFile.Read((uint)bRead), 0, fullData, 0, (int)bRead);
            bytesRead += (int)bRead;

            while (bytesRead < size)
            {
                ++mFileIndex;
                curFile = mStreamFiles[mFileIndex];
                bRead = Math.Min(curFile.FileSize, (size - bytesRead));
                Buffer.BlockCopy(curFile.Read((uint)bRead), 0, fullData, bytesRead, (int)bRead);
                bytesRead += (int)bRead;
            }

            mFileOffset = (int)mStreamFiles[mFileIndex].Position;

            Buffer.BlockCopy(fullData, 0, buffer, offset, count);

            Position = mOffsets[mFileIndex] + mFileOffset;
            return size;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    Position = offset;
                    break;

                case System.IO.SeekOrigin.Current:
                    Position += offset;
                    break;

                case System.IO.SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }

            return Position;
        }
        public override void Flush()
        {
        }
        public override long Position { get { return mFullPosition; } set { PositionChanged(value); } }
        public override long Length
        {
            get { return mOffsets.Last() + mFileSizes.Last(); }
        }
        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanTimeout { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }
        public override void Close()
        {
            base.Close();
        }
    }
}
