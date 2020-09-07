namespace NetTraceGen
{
    using System;
    using System.IO;
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

        public byte? Opcode;

        public Guid WindowsClassicEventProviderGuid;

        public FieldDefinition[] FieldDefinitions;

        private int v2ParamsTagBytes;

        internal override int GetPayloadSize()
        {
            // this line computes metadataId, providerName, eventId, eventName, keywords, version, level, v1fieldCount
            int size = sizeof(int) + (this.ProviderName.Length + 1) * 2 + sizeof(int) + (this.EventName.Length + 1) * 2 + sizeof(long) + sizeof(int) + sizeof(int) + sizeof(int);

            if (this.Opcode.HasValue)
            {
                // this line computes the tagPayloadBytes, tag type and the opcode
                size += sizeof(int) + sizeof(byte) + sizeof(byte);
            }

            if (this.WindowsClassicEventProviderGuid != Guid.Empty)
            {
                // this line computes the tagPayloadBytes, tag type and the guid
                size += sizeof(int) + sizeof(byte) + 16;
            }

            var fieldDefs = this.FieldDefinitions;
            var fieldDefsLength = fieldDefs.Length;

            if (fieldDefsLength > 0)
            {
                var tmp = sizeof(int); // v2 field count

                for (int i = 0; i < fieldDefsLength; ++i)
                {
                    tmp += fieldDefs[i].GetSerializationSize();
                }

                this.v2ParamsTagBytes = tmp;

                // this line computes the tagPayloadBytes, tag type, v2 param bytes
                size += sizeof(int) + sizeof(byte) + this.v2ParamsTagBytes;
            }

            return size;
        }

        protected override void SerializeBlob(Stream stream)
        {
            stream.WriteIntValue(this.MetadataId);

            var nullTerminator = new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 });

            stream.Write(MemoryMarshal.Cast<char, byte>(this.ProviderName.AsSpan()));
            stream.Write(nullTerminator);

            stream.WriteIntValue(this.EventId);

            stream.Write(MemoryMarshal.Cast<char, byte>(this.EventName.AsSpan()));
            stream.Write(nullTerminator);

            stream.WriteLongValue(this.Keywords);
            stream.WriteIntValue(this.Version);
            stream.WriteIntValue(this.Level);

            stream.WriteIntValue(0); // V1 Field Count, which we always put as 0

            if (this.Opcode.HasValue)
            {
                stream.WriteIntValue(1); // size of opcode
                stream.WriteByte(0x1); // 0x1 is tag kind opcode
                stream.WriteByte(this.Opcode.Value);
            }

            var fieldDefs = this.FieldDefinitions;
            var fieldDefsLength = fieldDefs.Length;
            if (fieldDefsLength > 0)
            {
                stream.WriteIntValue(this.v2ParamsTagBytes);
                stream.WriteByte(0x2); // 0x2 is tag kind v2 fields
                stream.WriteIntValue(fieldDefsLength); // v2 field count

                for (int i = 0; i < fieldDefsLength; ++i)
                {
                    stream.WriteIntValue(fieldDefs[i].GetSerializationSize());
                    fieldDefs[i].Serialize(stream);
                }
            }

            if (this.WindowsClassicEventProviderGuid != Guid.Empty)
            {
                stream.WriteIntValue(16); // size of guid
                stream.WriteByte(0x3); // 0x3 is tag kind windowss classic provider id
                stream.WriteGuidValue(ref this.WindowsClassicEventProviderGuid);
            }
        }
    }
}
