namespace NetTraceGen
{
    using System;
    using System.IO;
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

            stream.WriteIntValue(version);
            stream.WriteIntValue(minumumReaderVersion);
            stream.WriteIntValue(name.Length);
            stream.Write(Encoding.UTF8.GetBytes(name));

            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x6 }));
        }

        public void Dispose()
        {
            this.stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x6 }));
        }

        public void Align(int align)
        {
            this.stream.Position += (int)(0 - this.stream.Position & (align - 1));
        }

        public void WriteBytes(ReadOnlySpan<byte> data)
        {
            this.stream.Write(data);
        }

        public void WriteIntValue(int value)
        {
            this.stream.WriteIntValue(value);
        }

        public void WriteLongValue(long value)
        {
            this.stream.WriteLongValue(value);
        }

        public void WriteShortValue(int value)
        {
            this.stream.WriteShortValue(value);
        }
    }
}