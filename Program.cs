﻿namespace NetTraceGen
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

            var metadataEventBlobs = new[]
            {
                new MetadataEventBlob
                {
                    MetadataId = 1,
                    Level = 1,
                    Version = 0,
                    Keywords = 0,
                    EventName = "MyEventName",
                    ProviderName = "MyEventProvider",
                    EventId = 0,
                    Opcode = 35,
                    WindowsClassicEventProviderGuid = new Guid("9b79ee91-b5fd-41c0-a243-4248e266e9d0"),
                    EventHeader =
                    {
                        ActivityId = Guid.Empty,
                        RelatedActivityId = Guid.Empty,
                        CaptureThreadId = 1000,
                        ThreadId = 2000,
                        SequenceNumber = 1,
                        CaptureProcNumber = 10,
                        StackId = 0,
                        TimeStamp = Stopwatch.GetTimestamp()
                    },
                    FieldDefinitions = new[]
                    {
                        new FieldDefinition((int)TypeCode.String, 0, "MyFieldName1"),
                        new FieldDefinition((int)TypeCode.String, 0, "MyFieldName2")
                    }
                }
            };

            var metadataBlock = new EventBlock<MetadataEventBlob>("MetadataBlock", 2, 2, false, metadataEventBlobs);
            metadataBlock.Serialize(fs);

            var stringBlobs = new[]
            {
                new StringEventBlob("Hello World", "Something else kjdjks", 1),
                new StringEventBlob("Hello World", "Something else", 1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World","Something else", 1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1),
                new StringEventBlob("Hello World 2", "Something else",1),
                new StringEventBlob("Hello World", "Something else",1)
            };
            
            var eventBlock = new EventBlock<StringEventBlob>("EventBlock", 2, 2, true, stringBlobs);
            eventBlock.Serialize(fs);
            eventBlock.Serialize(fs);
            eventBlock.Serialize(fs);

            fs.Write(new ReadOnlySpan<byte>(new byte[] { 0x1 })); // NullReference Tag means EOF
        }
    }
}
