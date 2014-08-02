using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    static class FontTexture
    {
        public static Texture2D GenerateFontTexture(GraphicsDevice graphicsDevice, string text)
        {
            Font font = new Font("Arial Unicode MS", 18, FontStyle.Regular);

            Bitmap tmpmap = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(tmpmap);
            SizeF stringSize = g.MeasureString(text, font);


            Bitmap bitmap = new Bitmap((int)stringSize.Width, (int)stringSize.Height);
            g = Graphics.FromImage(bitmap);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawString(text, font, Brushes.Black, 0, 0, StringFormat.GenericDefault);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose();
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            return Texture2D.FromStream(graphicsDevice, ms);
        }
    }
}
