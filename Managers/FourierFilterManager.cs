using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ImageProcessingWeb.Models;

namespace ImageProcessingWeb.Managers
{
    public class FourierFilterManager
    {
        private readonly ColorManager _colorManager = new();

        public Bitmap ApplyFilter(Bitmap inputImage)
        {
            var width = inputImage.Width;
            var height = inputImage.Height;

            var complexArray = _colorManager.GetBitmapPixels(inputImage).Select(x => new ComplexRGB(x.R, x.G, x.B)).ToArray();
            var transformed = FourierTransform(complexArray, width, height, false);

            var radius = 20;
            for (var u = 0; u < width; u++)
                for (var v = 0; v < height; v++)
                {
                    var distance = Math.Sqrt(Math.Pow(u - width / 2, 2) + Math.Pow(v - height / 2, 2));
                    if (distance > radius) continue;
                    transformed[u + v * width] = new ComplexRGB(0, 0, 0);
                }
            
            var inverted = FourierTransform(transformed, width, height, true).Select(x => x / width / height);
            var result = _colorManager.DrawFromPixels(inverted.Select(x => x.ToColor()).ToList(), width);
            return result;
        }

        private ComplexRGB[] FourierTransform(ComplexRGB[] input, int width, int height, bool inverse)
        {
            var output = new ComplexRGB[width * height];

            for (var x1 = 0; x1 < width; x1++)
                for (var y1 = 0; y1 < height; y1++)
                {
                    var sum = new ComplexRGB(0, 0, 0);

                    for (var x2 = 0; x2 < width; x2++)
                        for (var y2 = 0; y2 < height; y2++)
                        {
                            var angle = 2 * Math.PI * (x1 * x2 / (double)width + y1 * y2 / (double)height);
                            var exp = new Complex(Math.Cos(angle), Math.Sin(angle) * (inverse ? 1 : -1));
                            sum += input[x2 + y2 * width] * exp;
                        }

                    output[x1 + y1 * width] = sum;
                }

            return output;
        }
    }
}
