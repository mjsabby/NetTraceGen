namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public sealed class StringEventBlob : EventBlob
    {
        private readonly string data1;

        private readonly string data2;

        public StringEventBlob(string data1, string data2, int metadataId)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.EventHeader.MetaDataId = metadataId;
        }

        internal override int GetPayloadSize() => (this.data1.Length + 1) * 2 + (this.data2.Length + 1) * 2;

        protected override void SerializeBlob(Stream stream)
        {
            stream.Write(MemoryMarshal.Cast<char, byte>(this.data1.AsSpan()));
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 }));

            stream.Write(MemoryMarshal.Cast<char, byte>(this.data2.AsSpan()));
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 }));
        }
    }
}
