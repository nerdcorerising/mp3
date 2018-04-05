using System;

namespace Mp3
{
    public class Mp3Frame
    {
        private Mp3FrameHeader header;
        private Mp3FrameData data;

        public Mp3Frame(Mp3FrameHeader header, Mp3FrameData data)
        {
            this.header = header;
            this.data = data;
        }
    }
}
