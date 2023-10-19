using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Console
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class HistogramManager
    {
        private const int Size = 1000;

        public Bitmap NormalizeHistogram(Bitmap image)
        {
            var pixels = GetBitmapPixels(image);
            var histogram = GetHistogram(Size, pixels);
            var cumulative = GetCumulativeHistogram(histogram);
            var equalized = Equalize(pixels, cumulative);
            return DrawFromPixels(equalized, image.Width);
        }

        public Bitmap GenerateHistogramImage(Bitmap image)
        {
            var pixels = GetBitmapPixels(image);
            var histogram = GetHistogram(Size, pixels);
            return DrawHistogram(Size, histogram);
        }

        private int[] GetHistogram(int size, List<Color> pixels)
        {
            var lights = new int[size];
            pixels.ForEach(x => lights[GetBrightness(x, size)]++);

            var max = lights.Max();
            return lights.Select(x => x * size / max).ToArray();
        }

        private int[] GetCumulativeHistogram(int[] histogram)
        {
            var cumulative = new int[histogram.Length];
            cumulative[0] = histogram[0];
            for (var i = 1; i < histogram.Length; i++)
                cumulative[i] = cumulative[i - 1] + histogram[i];
            return cumulative;
        }

        private List<Color> GetBitmapPixels(Bitmap bitmap)
        {
            var list = new List<Color>();
            for (var i = 0; i < bitmap.Height; i++)
                for (var j = 0; j < bitmap.Width; j++)
                    list.Add(bitmap.GetPixel(j, i));
            return list;
        }

        private Bitmap DrawFromPixels(List<Color> pixels, int width)
        {
            var height = pixels.Count / width;
            var bitmap = new Bitmap(width, height);
            for (var i = 0; i < pixels.Count; i++)
                bitmap.SetPixel(i % width, i / width, pixels[i]);

            return bitmap;
        }

        private Bitmap DrawHistogram(int size, int[] histogram)
        {
            var bitmap = new Bitmap(size, size);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, size, size);
            for (var i = 0; i < histogram.Length; i++)
            {
                var height = histogram[i];
                graphics.FillRectangle(Brushes.Black, i, Size - height, 1, height);
            }

            return bitmap;
        }

        private List<Color> Equalize(List<Color> pixels, IReadOnlyList<int> histogram)
        {
            float total = histogram[^1];
            for (var i = 0; i < pixels.Count; i++)
            {
                var part = GetBrightness(pixels[i], histogram.Count);
                var newBrightness = histogram[part] / total;
                var hue = pixels[i].GetHue();
                var saturation = pixels[i].GetSaturation();
                pixels[i] = ColorFromHSV(hue, saturation, newBrightness);
            }

            return pixels;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
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

        private int GetBrightness(Color color, int max) => Math.Min(max - 1, Convert.ToInt32(color.GetBrightness() * max));
    }
}
