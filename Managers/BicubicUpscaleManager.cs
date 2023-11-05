using System;
using System.Drawing;
using ImageProcessingWeb.Models;

namespace ImageProcessingWeb.Managers
{
    public class BicubicUpscaleManager
    {
        public Bitmap Upscale(Bitmap source)
        {
            var upscale = 2;
            var result = new Bitmap(source.Width * upscale, source.Height * upscale);
            for (var y = 0; y < result.Height; y++)
                for (var x = 0; x < result.Width; x++)
                    result.SetPixel(x, y, GetPixel(source, x, y, upscale));
            return result;
        }

        private Color GetPixel(Bitmap source, int x, int y, int upscale)
        {
            var ox = x / upscale;
            var oy = y / upscale;
            var dx = x % upscale;
            var dy = y % upscale;
            var code = BicubicInterpolationChannel(source, ox, oy, dx, dy, c => new RGB(c));
            return code.ToColor();
        }

        private RGB BicubicInterpolationChannel(Bitmap image, int x, int y, int dx, int dy, Func<Color, RGB> channelSelector)
        {
            var sum = new RGB();
            var totalWeight = 0d;

            for (var i = -1; i <= 2; i++)
                for (var j = -1; j <= 2; j++)
                {

                    var neighborX = (x + i) % image.Width;
                    var neighborY = (y + j) % image.Height;
                    if (neighborX < 0) neighborX += image.Width;
                    if (neighborY < 0) neighborY += image.Height;

                    var weightX = BicubicKernel(dx - i);
                    var weightY = BicubicKernel(dy - j);
                    var weight = weightX * weightY;

                    sum += channelSelector(image.GetPixel(neighborX, neighborY)) * weight;
                    totalWeight += weight;
                }

            return totalWeight == 0 ? new RGB() : sum / totalWeight;
        }
        private double BicubicKernel(double x)
        {
            x = Math.Abs(x);
            return x switch
            {
                <= 1 => 1.5 * Math.Pow(x, 3) - 2.5 * Math.Pow(x, 2) + 1,
                <= 2 => -0.5 * Math.Pow(x, 3) + 2.5 * Math.Pow(x, 2) - 4 * x + 2,
                _ => 0
            };
        }
    }
}