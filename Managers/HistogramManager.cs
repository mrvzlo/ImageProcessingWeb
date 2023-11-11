using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessingWeb.Managers
{
    public class HistogramManager
    {
        private const int Size = 1000;
        private readonly ColorManager _colorManager = new();

        public Bitmap NormalizeHistogram(Bitmap image)
        {
            var pixels = _colorManager.GetBitmapPixels(image);
            var histogram = GetHistogram(Size, pixels);
            var cumulative = GetCumulativeHistogram(histogram);
            var equalized = Equalize(pixels, cumulative);
            return _colorManager.DrawFromPixels(equalized, image.Width);
        }

        public Bitmap GenerateHistogramImage(Bitmap image)
        {
            var pixels = _colorManager.GetBitmapPixels(image);
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
                pixels[i] = _colorManager.ColorFromHSV(hue, saturation, newBrightness);
            }

            return pixels;
        }
        
        public int GetBrightness(Color color, int max = 1) => Math.Min(max - 1, Convert.ToInt32(_colorManager.GetValue(color) * max));
    }
}
