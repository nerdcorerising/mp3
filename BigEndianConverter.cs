using System;
using System.Runtime.InteropServices;

namespace Mp3
{
    internal class BigEndianConverter
    {
        internal static int ToInt32(byte[] buffer, int pos)
        {
            CheckLength<int>(buffer, pos);

            return (buffer[pos] << 24) | (buffer[pos + 1] << 16) | (buffer[pos + 2] << 8) | (buffer[pos + 3]);
        }

        internal static ushort ToUInt16(byte[] buffer, int pos)
        {
            CheckLength<short>(buffer, pos);

            return (ushort)((buffer[pos] << 8) | (buffer[pos + 1]));
        }

        internal static uint ToUInt32(byte[] buffer, int pos)
        {
            CheckLength<uint>(buffer, pos);
            return (uint)((buffer[pos] << 24) | (buffer[pos + 1] << 16) | (buffer[pos + 2] << 8) | (buffer[pos + 3]));
        }

        internal static ulong ToUInt64(byte[] buffer, int pos)
        {
            CheckLength<ulong>(buffer, pos);

            return (ulong)(((long)buffer[pos] << 56) | ((long)buffer[pos + 1] << 48) | ((long)buffer[pos + 2] << 40) | ((long)buffer[pos + 3] << 32)
                 | ((long)buffer[pos + 4] << 24) | ((long)buffer[pos + 5] << 16) | ((long)buffer[pos + 6] << 8) | ((long)buffer[pos + 7]));
        }

        private static void CheckLength<T>(byte[] buffer, int pos)
        {
            int bytesRequired = Marshal.SizeOf<T>();
            if (pos + bytesRequired > buffer.Length)
            {
                throw new ArgumentException($"Insufficient buffer for type {typeof(T)}.");
            }
        }
    }
}