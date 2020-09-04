namespace NetTraceGen
{
    using System.IO;
    using System.Runtime.CompilerServices;

    public sealed class EventBlock<T> where T : EventBlob
    {
        private readonly string blockName;

        private readonly int version;

        private readonly int miniumReaderVersion;

        private readonly T[] eventBlobs;

        public EventBlock(string blockName, int version, int miniumReaderVersion, T[] eventBlobs)
        {
            this.blockName = blockName;
            this.version = version;
            this.miniumReaderVersion = miniumReaderVersion;
            this.eventBlobs = eventBlobs;
        }

        public void Serialize(Stream stream)
        {
            using var obj = new FastSerializationObject(stream, this.blockName, this.version, this.miniumReaderVersion);

            int headerSize = 20; // TODO: compression will add size

            int blockSize = headerSize;

            for (int i = 0; i < this.eventBlobs.Length; ++i)
            {
                blockSize += this.eventBlobs[i].GetSerializationSize();
                blockSize += Padding(blockSize, 4);
            }

            obj.WriteIntValue(blockSize);
            obj.Align(4);

            obj.WriteShortValue(headerSize);
            obj.WriteShortValue(0);
            obj.WriteLongValue(0);
            obj.WriteLongValue(0);

            for (int i = 0; i < this.eventBlobs.Length; ++i)
            {
                this.eventBlobs[i].Serialize(stream);
                obj.Align(4);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Padding(long num, int align)
        {
            return (int)(0 - num & (align - 1));
        }
    }
}