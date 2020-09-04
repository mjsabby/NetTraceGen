namespace NetTraceGen
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public abstract class EventBlob
    {
        public NetTraceHeader EventHeader;

        public unsafe int GetSerializationSize() => sizeof(NetTraceHeader) + this.GetPayloadSize();

        public virtual void Serialize(Stream stream)
        {
            this.EventHeader.PayloadSize = this.GetPayloadSize();
            this.EventHeader.EventSize = this.GetSerializationSize() - sizeof(int);

            stream.Write(this.AsSpan(ref this.EventHeader));
        }

        internal abstract int GetPayloadSize();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySpan<byte> AsSpan<T>(ref T val) where T : unmanaged
        {
            return MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateReadOnlySpan(ref val, 1));
        }
    }
}