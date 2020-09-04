namespace NetTraceGen
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Diagnostics.Tracing;

    public static class Program
    {
        public static void Main(string[] args)
        {
            WriteNetPerf(args[0]);
            var source = new EventPipeEventSource(args[0]);
            if (source.Process())
            {
                Console.WriteLine("NetTrace file successfully written.");
            }
        }

        public static void WriteNetPerf(string filename)
        {
            var freq = Stopwatch.Frequency;
            var qpc = Stopwatch.GetTimestamp();
            var now = DateTime.Now;

            using var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);

            fs.Write(new ReadOnlySpan<byte>(new byte[] { (byte)'N', (byte)'e', (byte)'t', (byte)'t', (byte)'r', (byte)'a', (byte)'c', (byte)'e', 0x14, 0x0, 0x0, 0x0, (byte)'!', (byte)'F', (byte)'a', (byte)'s', (byte)'t', (byte)'S', (byte)'e', (byte)'r', (byte)'i', (byte)'a', (byte)'l', (byte)'i', (byte)'z', (byte)'a', (byte)'t', (byte)'i', (byte)'o', (byte)'n', (byte)'.', (byte)'1' }));

            var traceObject = new Trace(qpc, freq, in now, -1, IntPtr.Size, Environment.ProcessorCount, 1);
            traceObject.Serialize(fs);

            var metadataEventBlobs = new MetadataEventBlob[1];
            var fieldDefinitions = new FieldDefinition[1];
            fieldDefinitions[0] = new FieldDefinition(TypeCode.String, "MyFieldName");
            metadataEventBlobs[0] = new MetadataEventBlob
            {
                MetadataId = 1,
                Level = 1,
                Version = 1,
                Keywords = 0,
                EventName = "MyEventName",
                ProviderName = "MyEventProvider",
                EventId = 1,
                EventHeader =
                {
                    ActivityId = Guid.NewGuid(),
                    RelatedActivityId = Guid.NewGuid(),
                    CaptureThreadId = 1000,
                    ThreadId = 2000,
                    SequenceNumber = 1,
                    CaptureProcNumber = 10,
                    StackId = 0,
                    TimeStamp = Stopwatch.GetTimestamp()
                },
                FieldDefinitions = fieldDefinitions
            };

            var metadataBlock = new EventBlock<MetadataEventBlob>("MetadataBlock", 2, 2, metadataEventBlobs);
            metadataBlock.Serialize(fs);

            var stringBlobs = new StringEventBlob[2];
            stringBlobs[0] = new StringEventBlob("Hello World", 1);
            stringBlobs[1] = new StringEventBlob("Hello World 2", 1);

            var eventBlock = new EventBlock<StringEventBlob>("EventBlock", 2, 2, stringBlobs);
            eventBlock.Serialize(fs);

            fs.Write(new ReadOnlySpan<byte>(new byte[] { 0x1 })); // NullReference Tag means EOF
        }
    }
}
