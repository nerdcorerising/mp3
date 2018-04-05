using System;

namespace Mp3
{
    public class Mp3FrameHeader
    {
        // TODO: some space can be saved by changing these all to smaller data types
        private uint _syncWord;
        private uint _version;
        private uint _layer;
        private uint _bitRate;
        private uint _errorProtection;
        private uint _frequency;
        private uint _padBit;
        private uint _privateBit;
        private uint _mode;
        private uint _modeExtension;
        private uint _copy;
        private uint _original;
        private uint _emphasis;

        public Mp3FrameHeader(uint headerRaw)
        {
            _syncWord = (headerRaw & 0xFFF00000) >> 0x14;
            _version = (headerRaw & 0x00080000) >> 0x13;
            _layer = (headerRaw & 0x00060000) >> 0x11;
            _errorProtection = (headerRaw & 0x00010000) >> 0x10;
            _bitRate = (headerRaw & 0x0000F000) >> 0xC;
            _frequency = (headerRaw & 0x00000C00) >> 0xA;
            _padBit = (headerRaw & 0x00000200) >> 0x9;
            _privateBit = (headerRaw & 0x00000100) >> 0x8;
            _mode = (headerRaw & 0x000000C0) >> 0x6;
            _modeExtension = (headerRaw & 0x00000030) >> 0x4;
            _copy = (headerRaw & 0x00000008) >> 0x3;
            _original = (headerRaw & 0x00000004) >> 0x2;
            _emphasis = (headerRaw & 0x00000003);
        }

        public bool Valid
        {
            get
            {
                return _syncWord == 0xFFF;
            }
        }

        public int BitRate
        {
            get
            {
                switch (_bitRate)
                {
                    case 0b0000:
                        return 0;
                    case 0b0001:
                        return 32000;
                    case 0b0010:
                        return 40000;
                    case 0b0011:
                        return 48000;
                    case 0b0100:
                        return 56000;
                    case 0b0101:
                        return 64000;
                    case 0b0110:
                        return 80000;
                    case 0b0111:
                        return 96000;
                    case 0b1000:
                        return 112000;
                    case 0b1001:
                        return 128000;
                    case 0b1010:
                        return 160000;
                    case 0b1011:
                        return 192000;
                    case 0b1100:
                        return 224000;
                    case 0b1101:
                        return 256000;
                    case 0b1110:
                        return 320000;
                    case 0b1111:
                        throw new InvalidOperationException("Bad bitrate in frame header");
                    default:
                        throw new Exception("Shouldn't reach here");
                }
            }
        }

        public int Frequency
        {
            get
            {
                switch (_frequency)
                {
                    case 0b00:
                        return 44100;
                    case 0b01:
                        return 48000;
                    case 0b10:
                        return 32000;
                    case 0b11:
                        throw new InvalidOperationException("Illegal frequency in frame header");
                    default:
                        throw new Exception("Shouldn't reach here");
                }
            }
        }

        public bool Padding
        {
            get
            {
                return _padBit != 0;
            }
        }
    }
}