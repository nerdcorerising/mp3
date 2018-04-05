using System;
using System.Runtime.InteropServices;

namespace Mp3
{
    public class BigEndianConverter
    {
        public static int ToInt32(byte[] buffer, int pos)
        {
            return ConvertBytes<int>(buffer, pos);
        }

        public static ushort ToUInt16(byte[] buffer, int pos)
        {
            return ConvertBytes<ushort>(buffer, pos);
        }

        public static uint ToUInt32(byte[] buffer, int pos)
        {
            return ConvertBytes<uint>(buffer, pos);
        }

        public static ulong ToUInt64(byte[] buffer, int pos)
        {
            return ConvertBytes<ulong>(buffer, pos);
        }

        private static T ConvertBytes<T>(byte[] buffer, int pos)
        {
            int bytesRequired = Marshal.SizeOf<T>();
            CheckLength(bytesRequired, buffer, pos, typeof(T));

            long current = 0;
            int i = 0;
            while (bytesRequired >= 0)
            {
                bytesRequired -= 1;
                int bitShift = bytesRequired * 8;
                current |= (long)buffer[i] << bitShift;
                ++i;
            }

            return (T)Convert.ChangeType(current, typeof(T));
        }

        private static void CheckLength(int requiredBytes, byte[] buffer, int pos, Type t)
        {
            if (pos + requiredBytes > buffer.Length)
            {
                throw new ArgumentException($"Insufficient buffer for type {t}.");
            }
        }
    }
}
