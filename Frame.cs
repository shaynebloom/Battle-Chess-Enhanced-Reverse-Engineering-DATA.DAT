using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChessReverseEngineer
{
    public class Frame
    {
        private byte[] m_file;
        private int m_frameWidth, m_frameHeight, m_posX, m_posY, m_pixelOffset, m_pixelLength, m_pointer, m_thisOffset;
        private List<int> m_pixels = new List<int>();
        private String m_compression;

        public Frame(byte[] file, int frameWidth, int frameHeight, int posX, int posY,
            int pixelOffset, int pixelLength, string compression, int offset)
        {
            m_file = file;
            m_frameWidth = frameWidth;
            m_frameHeight = frameHeight;
            m_posX = posX;
            m_posY = posY;
            m_pixelOffset = pixelOffset;
            m_pixelLength = pixelLength;
            m_compression = compression;
            m_thisOffset = offset;

            if (m_compression.Equals("08"))
            {
                DecodeBytesLineCompression();
            }
            else if (m_compression.Equals("20"))
            {
                DecodeBytesCountCompression();
            }
            else if (m_compression.Equals("00"))
            {
                DecodeBytesNoCompression();
            }
        }

        // Getters, no setters
        public List<int> Pixels { get => m_pixels; }
        public int FrameWidth { get => m_frameWidth; }
        public int FrameHeight { get => m_frameHeight; }
        public int PosX { get => m_posX; }
        public int PosY { get => m_posY; }
        public int PixelOffset { get => m_pixelOffset; }
        public int PixelLength { get => m_pixelLength; }

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

        private void DecodeBytesLineCompression()
        {
            /*
             * Line type compression
		     * 
	    	 * This section is split into two, line information and line data section
	    	 * 
		     * Line information section is broken down into 4 byte long pieces.  Saves offset for first byte.  Then reads
		     * next 3 bytes, which represent how far in bytes the line data for this piece is from the first byte.  Fourth
		     * byte represents how many bytes make up the line.
		     * 
		     * It will then jump to the designated offset, and read the designated amount of bytes and store into pixel
		     * list.  Repeats until it reaches the end of the information section
		     */
            int currentPosition = m_pixelOffset,
                lineLocation;
            do
            {
                SetPointer(currentPosition);
                String b1 = BtS(ReadNext());
                String b2 = BtS(ReadNext());
                String b3 = BtS(ReadNext());
                String b4 = BtS(ReadNext());
                lineLocation = currentPosition + StI(b3 + b2 + b1);
                SetPointer(lineLocation);
                for (int i = 0; i < StI(b4); i++)
                {
                    m_pixels.Add(StI(BtS(ReadNext())));
                }
                currentPosition += 4;
            } while (currentPosition < (m_pixelOffset + m_pixelLength));
        }

        private void DecodeBytesCountCompression()
        {
            /*
             * Count compression (Some form of Run-Length Encoding ie RLE)
		     * 
		     * Pixel data mainly compressed with byte pairs.  The first byte represents how many times the pixel
		     * appears plus one, the next byte represents the pixel color.  So for example if the pair is 07 FF, then the 
		     * pixel FF will appear 7+1=8 times.
	    	 * 
		     * There is a secondary method to this though, if the first byte is greater than 150, then subtract from 256.
		     * Then take that many of the following bytes and add to pixel list.  For example if first byte is FE, then
		     * 256 - FE = 2, the next two bytes would be inserted into pixel list.  So FE FF FF would insert FF FF into
		     * the list.
	    	 */
            SetPointer(m_pixelOffset);
            do
            {
                int byte1 = StI(BtS(ReadNext()));
                if (byte1 > 150)
                {
                    byte1 = 256 - byte1;
                    for (int i = 0; i < byte1; i++)
                    {
                        m_pixels.Add(StI(BtS(ReadNext())));
                    }
                }
                else
                {
                    int colorByte = StI(BtS(ReadNext()));
                    for (int i = 0; i < byte1 + 1; i++)
                    {
                        m_pixels.Add(colorByte);
                    }
                }
            } while (m_pointer < (m_thisOffset + m_pixelOffset + m_pixelLength));
        }

        private void DecodeBytesNoCompression()
        {
            /* 
             * No compression
		     * 
		     * Bytes are read directly into the pixel list.
		     */
            SetPointer(m_pixelOffset);
            do
            {
                m_pixels.Add(StI(BtS(ReadNext())));
            } while (m_pointer < (m_thisOffset + m_pixelOffset + m_pixelLength));
        }
    }
}
