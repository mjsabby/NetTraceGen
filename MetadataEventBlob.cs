namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class MetadataEventBlob : EventBlob
    {
        public int MetadataId;

        public string ProviderName;

        public int EventId;

        public string EventName;

        public long Keywords;

        public int Version;

        public int Level;

        public FieldDefinition[] FieldDefinitions;

        internal override int GetPayloadSize()
        {
            int size = sizeof(int) + (this.ProviderName.Length + 1) * 2 + sizeof(int) + (this.EventName.Length + 1) * 2 + sizeof(long) + sizeof(int) + sizeof(int) + sizeof(int);

            for (int i = 0; i < this.FieldDefinitions.Length; ++i)
            {
                size += this.FieldDefinitions[i].GetSerializationSize();
            }

            return size;
        }

        public override void Serialize(Stream stream)
        {
            base.Serialize(stream);

            stream.Write(this.AsSpan(ref this.MetadataId));

            var nullTerminator = new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 });

            stream.Write(MemoryMarshal.Cast<char, byte>(this.ProviderName.AsSpan()));
            stream.Write(nullTerminator);

            stream.Write(this.AsSpan(ref this.EventId));

            stream.Write(MemoryMarshal.Cast<char, byte>(this.EventName.AsSpan()));
            stream.Write(nullTerminator);

            stream.Write(this.AsSpan(ref this.Keywords));
            stream.Write(this.AsSpan(ref this.Version));
            stream.Write(this.AsSpan(ref this.Level));

            var length = this.FieldDefinitions.Length;
            stream.Write(this.AsSpan(ref length));

            for (int i = 0; i < this.FieldDefinitions.Length; ++i)
            {
                this.FieldDefinitions[i].Serialize(stream);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySpan<byte> AsSpan<T>(ref T val) where T : unmanaged
        {
            return MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateReadOnlySpan(ref val, 1));
        }
    }
}
