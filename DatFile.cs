using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChessReverseEngineer
{
    class DatFile
    {
        private byte[] m_file;
        private List<int> m_offsets = new List<int>(), m_offsetSize = new List<int>();
        private int m_pointer;

        public DatFile(byte[] file)
        {
            m_file = file;
            m_pointer = 0;

            int savePointer;
            string[] dWord = new string[4];

            // Read first four bytes, which is offset for this chunk
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_offsets.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));

            // Read next four bytes, which is offset for second chunk containing chunk sizes
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_offsets.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));

            // Save current location and jump to second chunk to read size of first two chunks
            savePointer = m_pointer;
            m_pointer = m_offsets[1];

            // Read first four bytes, offset size for first chunk
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_offsetSize.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));

            // Read next four bytes, which is offset size for second chunk
            for (int i = 0; i < dWord.Length; i++)
            {
                dWord[i] = BtS(ReadNext());
            }
            m_offsetSize.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));

            // Now with size of second chunk, continue reading until end of chunk
            do
            {
                for (int i = 0; i < dWord.Length; i++)
                {
                    dWord[i] = BtS(ReadNext());
                }
                m_offsetSize.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));
            } while (m_pointer <
            (m_offsets[1] + m_offsetSize[1]));

            // Return to first chunk and continue reading now that you have that chunk's size as well
            m_pointer = savePointer;
            do
            {
                for (int i = 0; i < dWord.Length; i++)
                {
                    dWord[i] = BtS(ReadNext());
                }
                m_offsets.Add(StI(dWord[3] + dWord[2] + dWord[1] + dWord[0]));
            } while (m_pointer <
            (m_offsets[0] + m_offsetSize[0]));
        }

        // Getters, no setters
        public List<int> Offsets { get => m_offsets; }
        public List<int> OffsetSize { get => m_offsetSize; }

        private byte ReadNext()
        {
            return m_file[m_pointer++];
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
