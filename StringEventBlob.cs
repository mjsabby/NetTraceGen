namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public sealed class StringEventBlob : EventBlob
    {
        private readonly string data;

        public StringEventBlob(string data, int metadataId)
        {
            this.data = data;
            this.EventHeader.MetaDataId = metadataId;
        }

        internal override int GetPayloadSize() => (this.data.Length + 1) * 2;

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            stream.Write(MemoryMarshal.Cast<char, byte>(this.data.AsSpan()));
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 }));
        }
    }
}
