namespace NetTraceGen
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NetTraceHeader
    {
        public int EventSize;

        public int MetaDataId;

        public int SequenceNumber;

        public long ThreadId;

        public long CaptureThreadId;

        public int CaptureProcNumber;

        public int StackId;

        public long TimeStamp;

        public Guid ActivityId;

        public Guid RelatedActivityId;

        public int PayloadSize;
    }
}
