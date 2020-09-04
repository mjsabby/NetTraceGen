namespace NetTraceGen
{
    using System;
    using System.IO;

    public sealed class Trace
    {
        private readonly long qpc;

        private readonly long freq;

        private readonly DateTime now;

        private readonly int pid;

        private readonly int pointerSize;

        private readonly int processorCount;

        private readonly int samplingRate;

        public Trace(long qpc, long freq, in DateTime now, int pid, int pointerSize, int processorCount, int samplingRate)
        {
            this.qpc = qpc;
            this.freq = freq;
            this.now = now;
            this.pid = pid;
            this.pointerSize = pointerSize;
            this.processorCount = processorCount;
            this.samplingRate = samplingRate;
        }

        public void Serialize(Stream stream)
        {
            const int TraceVersion = 5;
            const int TraceMinumumReaderVersion = 5;

            using var obj = new FastSerializationObject(stream, "Trace", TraceVersion, TraceMinumumReaderVersion);

            obj.WriteShortValue(this.now.Year);
            obj.WriteShortValue(this.now.Month);
            obj.WriteShortValue((short)this.now.DayOfWeek);
            obj.WriteShortValue(this.now.Day);
            obj.WriteShortValue(this.now.Hour);
            obj.WriteShortValue(this.now.Minute);
            obj.WriteShortValue(this.now.Second);
            obj.WriteShortValue(this.now.Millisecond);
            obj.WriteLongValue(this.qpc);
            obj.WriteLongValue(this.freq);
            obj.WriteIntValue(this.pointerSize);
            obj.WriteIntValue(this.pid);
            obj.WriteIntValue(this.processorCount);
            obj.WriteIntValue(this.samplingRate);
        }
    }
}
