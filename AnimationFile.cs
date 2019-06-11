using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChessReverseEngineer
{
    public class AnimationFile
    {
        private byte[] m_file;
        private int m_frameDataOffset,      // Offset for frame data
                    m_animDataOffset,       // Offset for animation data
                    m_numOfFrames,          // Number of frames in this animation file
                    m_pointer,              // Pointer for reading bytes, used in readNext()
                    m_thisOffset,           // Offset for this chunk
                    m_fileSize;             // Size in bytes of this chunk
        private Frame[] m_frames;           // Array of frames, to be constructed

        public AnimationFile(byte[] file, int offset, int size)
        {
            m_file = file;
            m_thisOffset = offset;
            m_fileSize = size;
            SetPointer(0);                  // Set pointer to beginning of chunk
            String[] dWord = new string[4]; // 4 String array for doing stuff with bytes

            // Read first 4 bytes, which are frame data offset in little endian.
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_frameDataOffset = StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]);

            // Read second 4 bytes, which are animation data offset in little endian
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_animDataOffset = StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]);

            // Caluclate number of frames.  First 15 bytes of frame data are not important, so subtract 16
            m_numOfFrames = (m_animDataOffset - m_frameDataOffset - 16) / 16;

            // Create array of frames
            m_frames = new Frame[m_numOfFrames];

            // Set pointer to look at frame data section, skipping first 16 bytes
            SetPointer(m_frameDataOffset + 16);

            // For loop for initializing frames
            for (int i = 0; i < m_frames.Length; i++)
            {
                String frameWidth, frameHeight, posX, posY, pixelOffset, pixelLength, b1, b2, b3, b4;

                // First two bytes are width in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                frameWidth = b2 + b1;
                
                // Second two bytes are height in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                frameHeight = b2 + b1;
                
                // Some type of x coordinate in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                posX = b2 + b1;
                
                // Some type of y coordinate in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                posY = b2 + b1;
                
                // Offset for where pixel data begins in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                b3 = BtS(ReadNext());
                b4 = BtS(ReadNext());
                pixelOffset = b4 + b3 + b2 + b1;

                // Pixel data length in bytes in little endian
                b1 = BtS(ReadNext());
                b2 = BtS(ReadNext());
                b3 = BtS(ReadNext());
                pixelLength = b3 + b2 + b1;
                
                // Last byte represents how file is compressed.  Used by FrameData class to determine how to decompress
                String compression = BtS(ReadNext());

                m_frames[i] = new Frame(m_file,
                    StI(frameWidth),
                    StI(frameHeight),
                    StI(posX),
                    StI(posY),
                    StI(pixelOffset),
                    StI(pixelLength),
                    compression,
                    m_thisOffset);
            }
        }

        // Getters, no setters
        public int FrameDataOffset { get => m_frameDataOffset; }
        public int AnimDataOffset { get => m_animDataOffset; }
        public int NumOfFrames { get => m_numOfFrames; }

        public int GetPixel(int frameNo, int PixelNo)
        {
            return m_frames[frameNo].Pixels[PixelNo];
        }

        public int getFrameWidth(int frameNo)
        {
            return m_frames[frameNo].FrameWidth;
        }

        public int getFrameHeight(int frameNo)
        {
            return m_frames[frameNo].FrameHeight;
        }

        private byte ReadNext()                     // Helper method, reads and returns byte at pointer, then increments pointer
        {
            return m_file[m_pointer++];
        }

        private void SetPointer(int point)          // Helper method to read bytes, calculates position based of chunk offset
        {
            m_pointer = point + m_thisOffset;
        }

        private string BtS(byte b)                  // Byte to String helper function
        {
            return BitConverter.ToString(new[] { b });
        }

        private int StI(string s)                   // String to Int, helper function to clean up code
        {
            return Int32.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
