using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace netLogic.Utilities
{
    public static class Memory
    {
        public static void CopyMemory<T, U>(T[] src, U[] dst)
        {
            GCHandle srcHandle = GCHandle.Alloc(src, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(dst, GCHandleType.Pinned);

            MoveMemory(dstHandle.AddrOfPinnedObject(), srcHandle.AddrOfPinnedObject(), src.Length * Marshal.SizeOf(typeof(T)));
            srcHandle.Free();
            dstHandle.Free();
        }

        public static void CopyMemory<T, U>(T src, U dst, int numBytes = -1)
        {
            GCHandle srcHandle = GCHandle.Alloc(src, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(dst, GCHandleType.Pinned);

            MoveMemory(dstHandle.AddrOfPinnedObject(), srcHandle.AddrOfPinnedObject(), (numBytes == -1) ? Marshal.SizeOf(typeof(T)) : numBytes);
        }

        public static void CopyMemory<T>(T[] src, IntPtr dst)
        {
            GCHandle srcHandle = GCHandle.Alloc(src, GCHandleType.Pinned);

            MoveMemory(dst, srcHandle.AddrOfPinnedObject(), src.Length * Marshal.SizeOf(typeof(T)));

            srcHandle.Free();
        }

        public static void CopyMemory(IntPtr src, IntPtr dst, int numBytes)
        {
            MoveMemory(dst, src, numBytes);
        }

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);
    }
}
