namespace NetTraceGen
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;

    public sealed class EventBlock<T> where T : EventBlob
    {
        private readonly string blockName;

        private readonly int version;

        private readonly int miniumReaderVersion;

        private readonly bool compress;

        private readonly T[] eventBlobs;

        public EventBlock(string blockName, int version, int miniumReaderVersion, bool compress, T[] eventBlobs)
        {
            this.blockName = blockName;
            this.version = version;
            this.miniumReaderVersion = miniumReaderVersion;
            this.compress = compress;
            this.eventBlobs = eventBlobs;
        }

        public void Serialize(Stream stream)
        {
            if (this.compress)
            {
                this.CompressAndSerialize(stream);
            }
            else
            {
                using var obj = new FastSerializationObject(stream, this.blockName, this.version, this.miniumReaderVersion);

                const int headerSize = 20;

                int blockSize = headerSize;

                for (int i = 0; i < this.eventBlobs.Length; ++i)
                {
                    blockSize += this.eventBlobs[i].GetSerializationSize();
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
        }

        private void CompressAndSerialize(Stream stream)
        {
            using var obj = new FastSerializationObject(stream, this.blockName, this.version, this.miniumReaderVersion);

            var arrayPool = ArrayPool<byte>.Shared;
            byte[] compressionMemory = null;
            byte[] outputMemory = null;
            try
            {
                const int OneMB = 1024 * 1024;

                compressionMemory = arrayPool.Rent(OneMB);

                var compressionStream = new MemoryStream(compressionMemory, 0, OneMB);

                for (int i = 0; i < this.eventBlobs.Length; ++i)
                {
                    this.eventBlobs[i].Serialize(compressionStream);
                }

                outputMemory = arrayPool.Rent(1024 * 1024); // !MB

                var decompressedSize = (int)compressionStream.Position;
                var compressedSize = ULZCompression.Compress(new ArraySegment<byte>(compressionMemory, 0, (int)compressionStream.Position), outputMemory, 2);

                const int headerSize = 24;
                obj.WriteIntValue(headerSize + compressedSize);
                obj.Align(4);

                obj.WriteShortValue(headerSize);
                obj.WriteShortValue(0x2); // 2 means ULZCompression
                obj.WriteLongValue(0);
                obj.WriteLongValue(0);
                obj.WriteIntValue(decompressedSize);
                obj.WriteBytes(new ReadOnlySpan<byte>(outputMemory, 0, compressedSize));
            }
            finally
            {
                if (compressionMemory != null)
                {
                    arrayPool.Return(compressionMemory);
                }

                if (outputMemory != null)
                {
                    arrayPool.Return(outputMemory);
                }
            }
        }
    }
}