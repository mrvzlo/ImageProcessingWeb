using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessingWeb.Managers
{
    public class ColorManager
    {
        public List<Color> GetBitmapPixels(Bitmap bitmap)
        {
            var list = new List<Color>();
            for (var i = 0; i < bitmap.Height; i++)
            for (var j = 0; j < bitmap.Width; j++)
                list.Add(bitmap.GetPixel(j, i));
            return list;
        }

        public Bitmap DrawFromPixels(List<Color> pixels, int width)
        {
            var height = pixels.Count / width;
            var bitmap = new Bitmap(width, height);
            for (var i = 0; i < pixels.Count; i++)
                bitmap.SetPixel(i % width, i / width, pixels[i]);

            return bitmap;
        }

        public Color ColorFromHSV(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(255, v, t, p),
                1 => Color.FromArgb(255, q, v, p),
                2 => Color.FromArgb(255, p, v, t),
                3 => Color.FromArgb(255, p, q, v),
                4 => Color.FromArgb(255, t, p, v),
                _ => Color.FromArgb(255, v, p, q)
            };
        }


        public double GetValue(Color color) => Math.Max(color.R, Math.Max(color.G, color.B)) / 255d;
    }
}
