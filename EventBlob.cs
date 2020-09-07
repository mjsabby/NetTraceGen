namespace NetTraceGen
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public abstract class EventBlob
    {
        public NetTraceHeader EventHeader;

        public unsafe int GetSerializationSize()
        {
            var tmp = sizeof(NetTraceHeader) + this.GetPayloadSize();
            tmp += Padding(tmp, 4);

            return tmp;
        }

        protected abstract void SerializeBlob(Stream stream);

        public void Serialize(Stream stream)
        {
            this.EventHeader.PayloadSize = this.GetPayloadSize();
            this.EventHeader.EventSize = this.GetSerializationSize() - sizeof(int);
            stream.Write(MemoryMarshal.Cast<NetTraceHeader, byte>(MemoryMarshal.CreateReadOnlySpan(ref this.EventHeader, 1)));

            this.SerializeBlob(stream);

            stream.Position += Padding(stream.Position, 4);
        }

        internal abstract int GetPayloadSize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Padding(long num, int align)
        {
            return (int)(0 - num & (align - 1));
        }
    }
}