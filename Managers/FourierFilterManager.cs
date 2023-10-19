using System;
using System.Drawing;
using System.Numerics;

namespace Console
{
    public class FourierFilterManager
    {
        public Bitmap ApplyFilter(Bitmap inputImage)
        {
            var width = inputImage.Width;
            var height = inputImage.Height;

            var complexArray = TransformImageToComplexArray(inputImage);
            var transformed = FourierTransform(complexArray, width, height, false);

            var radius = 20;
            for (var u = 0; u < width; u++)
                for (var v = 0; v < height; v++)
                {
                    var distance = Math.Sqrt(Math.Pow(u - width / 2, 2) + Math.Pow(v - height / 2, 2));
                    if (distance > radius) continue;
                    transformed[u, v] = new Complex(0, 0);
                }
            
            var inverted = FourierTransform(transformed, width, height, true);
            var result = ConvertToBitmap(inverted);
            return result;
        }

        private Complex[,] TransformImageToComplexArray(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;
            var complexArray = new Complex[width, height];

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    var real = pixel.GetBrightness() * 255;
                    complexArray[x, y] = new Complex(real, 0);
                }

            return complexArray;
        }

        private Complex[,] FourierTransform(Complex[,] input, int width, int height, bool inverse)
        {
            var output = new Complex[width, height];

            for (var x1 = 0; x1 < width; x1++)
                for (var y1 = 0; y1 < height; y1++)
                {
                    var sum = new Complex(0, 0);

                    for (var x2 = 0; x2 < width; x2++)
                        for (var y2 = 0; y2 < height; y2++)
                        {
                            var angle = 2 * Math.PI * (x1 * x2 / (double)width + y1 * y2 / (double)height);
                            var exp = new Complex(Math.Cos(angle), Math.Sin(angle) * (inverse ? 1 : -1));
                            sum += input[x2, y2] * exp;
                        }

                    output[x1, y1] = sum;
                }

            return output;
        }

        private Bitmap ConvertToBitmap(Complex[,] grayscaleImage)
        {
            var width = grayscaleImage.GetLength(0);
            var height = grayscaleImage.GetLength(1);
            var bitmap = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var grayValue = (byte)(grayscaleImage[x, y].Magnitude / width / height);
                    var pixel = Color.FromArgb(grayValue, grayValue, grayValue);
                    bitmap.SetPixel(x, y, pixel);
                }

            return bitmap;
        }
    }
}
