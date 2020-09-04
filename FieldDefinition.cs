namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public sealed class FieldDefinition
    {
        private readonly int typeCode;

        private readonly string fieldName;

        public FieldDefinition(TypeCode typeCode, string fieldName)
        {
            this.typeCode = (int)typeCode;
            this.fieldName = fieldName;
        }

        public int GetSerializationSize()
        {
            return sizeof(int) + (this.fieldName.Length + 1) * 2;
        }

        public void Serialize(Stream stream)
        {
            Span<byte> intValue = stackalloc byte[4];
            BitConverter.TryWriteBytes(intValue, this.typeCode);
            stream.Write(intValue);

            stream.Write(MemoryMarshal.Cast<char, byte>(this.fieldName.AsSpan()));
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 }));
        }
    }
}