using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChessReverseEngineer
{
    class ColorPalette
    {
        /*
         * Color palettes were manually created using the palettes from the .RIX files.  The game
         * would somehow flip the palettes between red and blue, so there are two palette files,
         * red.dat and blue.dat, one for each.
         */
        private List<int[]> m_palette = new List<int[]>();
        
        public ColorPalette(string filePath)
        {
            byte[] file = File.ReadAllBytes(filePath);
            int pointer = 0;
            for (int i = 0; i < 256; i++)
            {
                int r, g, b;
                r = StI(BtS(file[pointer++]));
                g = StI(BtS(file[pointer++]));
                b = StI(BtS(file[pointer++]));

                r <<= 2; // Colors are bit shifted two to the right, so shift them two to the left to restore
                g <<= 2;
                b <<= 2;

                int[] color = { r, g, b };
                m_palette.Add(color);
            }
        }

        public int GetR(int index)
        {
            return m_palette[index][0];
        }

        public int GetG(int index)
        {
            return m_palette[index][1];
        }

        public int GetB(int index)
        {
            return m_palette[index][2];
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
