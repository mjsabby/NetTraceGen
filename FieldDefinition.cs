namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public sealed class FieldDefinition
    {
        private readonly int typeCode;

        private readonly int arrayElementTypeCode;

        private readonly string fieldName;

        public FieldDefinition(int typeCode, int arrayElementTypeCode, string fieldName)
        {
            if (typeCode == 1)
            {
                throw new Exception("TypeCode 1 is the Object Type Code, which is not yet supported.");
            }

            if (typeCode == 19 && arrayElementTypeCode == 0)
            {
                throw new Exception("TypeCode 19 is the Array Type Code, and it needs an element type code that is valid.");
            }

            this.typeCode = typeCode;
            this.arrayElementTypeCode = arrayElementTypeCode;
            this.fieldName = fieldName;
        }

        public int GetSerializationSize()
        {
            // this line computes fieldLength, fieldName, typeCode, optional array type code
            return sizeof(int) + (this.fieldName.Length + 1) * 2  + sizeof(int) + (this.arrayElementTypeCode == 0 ? 0 : sizeof(int));
        }

        public void Serialize(Stream stream)
        {
            stream.Write(MemoryMarshal.Cast<char, byte>(this.fieldName.AsSpan()));
            stream.Write(new ReadOnlySpan<byte>(new byte[] { 0x0, 0x0 }));
            stream.WriteIntValue(this.typeCode);

            if (this.arrayElementTypeCode != 0)
            {
                stream.WriteIntValue(this.arrayElementTypeCode);
            }
        }
    }
}