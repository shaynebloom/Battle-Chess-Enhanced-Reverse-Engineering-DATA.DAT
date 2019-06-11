using System.Drawing;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleChessReverseEngineer
{
    class CreateBitmaps
    {
        private static ColorPalette m_redPalette, m_bluePalette;
        //AnimationFile m_animFile;
        //private int m_offset;

        static CreateBitmaps()
        {
            m_redPalette = new ColorPalette("red.dat");
            m_bluePalette = new ColorPalette("blue.dat");
        }

        public static void GeneratePNG(byte[] file, int offset, int size)
        {
            AnimationFile animFile = new AnimationFile(file, offset, size);

            for (int i = 0; i < animFile.NumOfFrames; i++)
            {
                Bitmap bmpRed = new Bitmap(animFile.getFrameWidth(i), animFile.getFrameHeight(i));
                Bitmap bmpBlue = new Bitmap(animFile.getFrameWidth(i), animFile.getFrameHeight(i));

                int pixelNo = 0;
                int a = 255;
                for (int y = 0; y < animFile.getFrameHeight(i); y++)
                {
                    for (int x = 0; x < animFile.getFrameWidth(i); x++)
                    {
                        int rr = m_redPalette.GetR(animFile.GetPixel(i, pixelNo));
                        int gr = m_redPalette.GetG(animFile.GetPixel(i, pixelNo));
                        int br = m_redPalette.GetB(animFile.GetPixel(i, pixelNo));
                        int rb = m_bluePalette.GetR(animFile.GetPixel(i, pixelNo));
                        int gb = m_bluePalette.GetG(animFile.GetPixel(i, pixelNo));
                        int bb = m_bluePalette.GetB(animFile.GetPixel(i, pixelNo++));
                        if (rr == 248 && gr == 248 && br == 248)
                        {
                            a = 0;
                        }
                        else
                        {
                            a = 255;
                        }
                        bmpRed.SetPixel(x, y, Color.FromArgb(a, rr, gr, br));
                        bmpBlue.SetPixel(x, y, Color.FromArgb(a, rb, gb, bb));
                    }
                }
                Directory.CreateDirectory(@"red\" + offset);
                Directory.CreateDirectory(@"blue\" + offset);
                bmpRed.Save(@"red\" + offset + "\\" + i + ".png");
                bmpBlue.Save(@"blue\" + offset + "\\" + i + ".png");
            }
            

            
        }
    }
}
