namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal static class Extensions
    {
        public static void Align(this Stream stream, int align)
        {
            stream.Position += (int)(0 - stream.Position & (align - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteShortValue(this Stream stream, int value)
        {
            Span<byte> shortValue = stackalloc byte[2];
            BitConverter.TryWriteBytes(shortValue, (short)value);
            stream.Write(shortValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteIntValue(this Stream stream, int value)
        {
            Span<byte> intValue = stackalloc byte[4];
            BitConverter.TryWriteBytes(intValue, value);
            stream.Write(intValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLongValue(this Stream stream, long value)
        {
            Span<byte> longValue = stackalloc byte[8];
            BitConverter.TryWriteBytes(longValue, value);
            stream.Write(longValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteGuidValue(this Stream stream, ref Guid value)
        {
            Span<byte> guidValue = stackalloc byte[16];
            value.TryWriteBytes(guidValue);
            stream.Write(guidValue);
        }
    }
}
