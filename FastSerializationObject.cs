namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal readonly ref struct FastSerializationObject
    {
        private readonly Stream stream;

        public FastSerializationObject(Stream stream, string name, int version, int minumumReaderVersion)
        {
            var beginPrivateObject = new ReadOnlySpan<byte>(new byte[] { 0x5 });

            this.stream = stream;

            stream.Write(beginPrivateObject);

            stream.Write(beginPrivateObject);
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x1 }));

            WriteIntValue(stream, version);
            WriteIntValue(stream, minumumReaderVersion);
            WriteIntValue(stream, name.Length);
            stream.Write(Encoding.UTF8.GetBytes(name));

            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x6 }));

            static void WriteIntValue(Stream stream, int value)
            {
                Span<byte> intValue = stackalloc byte[4];
                BitConverter.TryWriteBytes(intValue, value);
                stream.Write(intValue);
            }
        }

        public void Dispose()
        {
            this.stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x6 }));
        }

        public void Align(int align)
        {
            this.stream.Position += (int)(0 - this.stream.Position & (align - 1));
        }

        public void WriteIntValue(int value)
        {
            WriteIntValue(this.stream, value);
        }

        public void WriteLongValue(long value)
        {
            WriteLongValue(this.stream, value);
        }

        public void WriteShortValue(int value)
        {
            WriteShortValue(this.stream, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteShortValue(Stream stream, int value)
        {
            Span<byte> shortValue = stackalloc byte[2];
            BitConverter.TryWriteBytes(shortValue, (short)value);
            stream.Write(shortValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteIntValue(Stream stream, int value)
        {
            Span<byte> intValue = stackalloc byte[4];
            BitConverter.TryWriteBytes(intValue, value);
            stream.Write(intValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteLongValue(Stream stream, long value)
        {
            Span<byte> longValue = stackalloc byte[8];
            BitConverter.TryWriteBytes(longValue, value);
            stream.Write(longValue);
        }
    }
}