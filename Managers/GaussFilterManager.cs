using System;
using System.Drawing;

namespace Console
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class GaussFilterManager
    {
        public Bitmap ApplyFilter(Bitmap source, int sigma)
        {
            var width = source.Width;
            var height = source.Height;
            var result = new Bitmap(width, height);
            var kernel = CreateKernel(sigma);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    result.SetPixel(x, y, CalculatePixel(source, x, y, sigma, kernel));

            return result;
        }

        private Color CalculatePixel(Bitmap image, int x, int y, int sigma, double[,] kernel)
        {
            var width = image.Width;
            var height = image.Height;
            double r = 0, g = 0, b = 0;
            for (var dx = -sigma; dx <= sigma; dx++)
                for (var dy = -sigma; dy <= sigma; dy++)
                {
                    var neighborX = (x + dx) % width;
                    var neighborY = (y + dy) % height;
                    if (neighborX < 0) neighborX += width;
                    if (neighborY < 0) neighborY += height;

                    var neighborPixel = image.GetPixel(neighborX, neighborY);
                    var weight = kernel[dx + sigma, dy + sigma];
                    r += neighborPixel.R * weight;
                    g += neighborPixel.G * weight;
                    b += neighborPixel.B * weight;
                }

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private double[,] CreateKernel(int sigma)
        {
            var size = sigma * 2 + 1;
            var kernel = new double[size, size];
            var sum = 0d;

            for (var x = 0; x < size; x++)
                for (var y = 0; y < size; y++)
                {
                    kernel[x, y] = GetWeight(x - sigma, y - sigma, sigma);
                    sum += kernel[x, y];
                }

            for (var x = 0; x < size; x++)
                for (var y = 0; y < size; y++)
                    kernel[x, y] /= sum;

            return kernel;
        }

        private double GetWeight(int x, int y, int sigma) => Math.Exp(-(x * x + y * y) / (2d * sigma * sigma));
    }
}
