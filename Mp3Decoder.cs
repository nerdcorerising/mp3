using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Mp3
{
    public class Mp3Decoder
    {
        private bool _done = false;
        private Stream _input = null;
        private byte[] _buffer = new byte[4096];
        private int _currentBufferPos = 0;
        private int _currentBufferSize = 0;
        private int _totalBytesRead = 0;
        private byte[] _conversionBuffer = new byte[128];

        public Mp3Decoder(Stream input)
        {
            _input = input;

            if (HasID3Tag())
            {
                SkipID3Tag();
            }
        }

        public IEnumerable<Mp3Frame> GetFrames()
        {
            while (!EndOfInput())
            {
                yield return ParseFrame();
            }
        }

        private bool HasID3Tag()
        {
            PeekBytes(_conversionBuffer, 3);

            return _conversionBuffer[0] == (byte)'I'
                && _conversionBuffer[1] == (byte)'D'
                && _conversionBuffer[2] == (byte)'3';
        }

        private void SkipID3Tag()
        {
            ReadBytes(_conversionBuffer, 10);
            // Convert from Synchsafe integer to real int
            int size = 0;
            for (int i = 6; i < 10; ++i)
            {
                size |= ((_conversionBuffer[i] & 0x7f) << ((9 - i) * 7));
            }

            SkipBytes(size);
        }

        private void SkipBytes(int count)
        {
            while (count > 0)
            {
                EnsureBytesAvailable(Math.Min(_buffer.Length, count));

                int bytesAvailable = _currentBufferSize - _currentBufferPos;
                int bytesAdvanced = count > bytesAvailable ? bytesAvailable : count;
                _currentBufferPos += bytesAdvanced;
                count -= bytesAdvanced;
            }
        }

        private void PeekBytes(byte[] outBuffer, int count)
        {
            if (count >= outBuffer.Length)
            {
                throw new ArgumentException($"Requested {count} bytes, but buffer is only {outBuffer.Length} bytes in size.");
            }

            EnsureBytesAvailable(count);

            for (int i = 0; i < count; ++i)
            {
                outBuffer[i] = _buffer[_currentBufferPos + i];
            }
        }

        private void ReadBytes(byte[] outBuffer, int count)
        {
            if (count > outBuffer.Length)
            {
                throw new ArgumentException($"Requested {count} bytes, but buffer is only {outBuffer.Length} bytes in size.");
            }

            EnsureBytesAvailable(count);
            
            for(int i = 0; i < count; ++i)
            {
                outBuffer[i] = _buffer[_currentBufferPos];
                _currentBufferPos++;
            }
        }

        private void EnsureBytesAvailable(int count)
        {
            if ((_currentBufferPos + count) > _currentBufferSize)
            {
                RefillBuffer();

                if ((_currentBufferPos + count) > _currentBufferSize)
                {
                    // TODO: make this a better exception type and message
                    throw new Exception($"Tried to read {count} bytes but only {_currentBufferSize - _currentBufferPos} bytes are available.");
                }
            }
        }

        private void RefillBuffer()
        {
            int bytesLeft = _currentBufferSize - _currentBufferPos;
            int bytesToRead = _buffer.Length - bytesLeft;

            for(int i = 0; i < bytesLeft; ++i)
            {
                _buffer[i] = _buffer[_currentBufferPos + i];
            }

            _currentBufferPos = 0;

            int bytesRead =_input.Read(_buffer, bytesLeft, bytesToRead);
            _totalBytesRead += bytesRead;
            _currentBufferSize = bytesRead + bytesLeft;

            if (bytesRead == 0)
            {
                _done = true;
            }
        }

        private byte ReadNextByte()
        {
            ReadBytes(_conversionBuffer, 1);
            return _conversionBuffer[0];
        }

        private ushort ReadNextShort()
        {
            ReadBytes(_conversionBuffer, 2);
            return BigEndianConverter.ToUInt16(_conversionBuffer, 0);
        }

        private uint ReadNextInt()
        {
            ReadBytes(_conversionBuffer, 4);
            return BigEndianConverter.ToUInt32(_conversionBuffer, 0);
        }

        private ulong ReadNextLong()
        {
            ReadBytes(_conversionBuffer, 8);
            return BigEndianConverter.ToUInt64(_conversionBuffer, 0);
        }

        private Mp3Frame ParseFrame()
        {
            Mp3FrameHeader header = ParseHeader();

            Mp3FrameData data = null;
            if(header.Valid)
            {
                data = ParseData(header);
            }
            else
            {
                _done = true;
            }

            return new Mp3Frame(header, data);
        }

        private Mp3FrameData ParseData(Mp3FrameHeader header)
        {
            int padding = header.Padding ? 1 : 0;
            int frameSize = ((144 * header.BitRate) / header.Frequency) + padding;
            // subtract the header length
            frameSize -= 4;
            byte[] data = new byte[frameSize];
            ReadBytes(data, frameSize);

            return new Mp3FrameData(data);
        }

        private Mp3FrameHeader ParseHeader()
        {
            uint headerRaw = ReadNextInt();
            return new Mp3FrameHeader(headerRaw);
        }

        private bool EndOfInput()
        {
            if (_currentBufferPos == _currentBufferSize)
            {
                RefillBuffer();
            }

            return _done && _currentBufferPos >= _currentBufferSize;
        }
    }
}
