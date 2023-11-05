using System.Drawing;
using System.Linq;
using ImageProcessingWeb.Models;

namespace ImageProcessingWeb.Managers
{
    public class HaarNoiseReductionManager
    {
        private readonly ColorManager _colorManager = new();

        public Bitmap Denoise(Bitmap image, double threshold)
        {
            var pixels = _colorManager.GetBitmapPixels(image).Select(x => new RGB(x)).ToArray();
            for (var index = 0; index < pixels.Length; index++)
                pixels[index] -= 127; // Haar transformation is for values from -1 to 1, so we make a shift to be from -127 to 128

            for (var i = 0; i < 2; i++) // One iteration make the whole image grayish so minimum 2, the more iterations - the more intensivity
                pixels = TransformHaarMatrixForward(pixels, image.Width, image.Height);
            ReduceNoise(pixels, threshold);
            for (var i = 0; i < 2; i++)
                pixels = TransformHaarMatrixInverse(pixels, image.Width, image.Height);

            for (var index = 0; index < pixels.Length; index++)
                pixels[index] += 127;
            return _colorManager.DrawFromPixels(pixels.Select(x => x.ToColor()).ToList(), image.Width);
        }

        private RGB[] TransformHaarMatrixForward(RGB[] pixels, int width, int height)
        {
            for (var i = 0; i < height; i++)
                TransformHaarArrayForward(pixels, i * width, width);
            // Rotate image to process columns as rows and then rollback
            pixels = Transpose(pixels, width, height);
            for (var i = 0; i < width; i++)
                TransformHaarArrayForward(pixels, i * height, height);
            pixels = Transpose(pixels, height, width);
            return pixels;
        }

        private RGB[] TransformHaarMatrixInverse(RGB[] pixels, int width, int height)
        {
            for (var i = 0; i < width; i++)
                TransformHaarArrayInverse(pixels, i * height, height);
            pixels = Transpose(pixels, height, width);
            for (var i = 0; i < height; i++)
                TransformHaarArrayInverse(pixels, i * width, width);
            pixels = Transpose(pixels, width, height);
            return pixels;
        }

        private void ReduceNoise(RGB[] pixels, double threshold)
        {
            foreach (var pixel in pixels)
            {
                pixel.R = ReduceNoiseChannel(pixel.R, threshold);
                pixel.G = ReduceNoiseChannel(pixel.G, threshold);
                pixel.B = ReduceNoiseChannel(pixel.B, threshold);
            }
        }

        private double ReduceNoiseChannel(double value, double threshold)
        {
            if (value > threshold) return value - threshold;
            if (value < -threshold) return value + threshold;
            return 0;
        }

        private RGB[] Transpose(RGB[] data, int width, int height)
        {
            var transposed = new RGB[data.Length];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    transposed[x * height + y] = data[y * width + x];
            return transposed;
        }

        public void TransformHaarArrayForward(RGB[] sourceList, int start, int length)
        {
            var temp = new RGB[length];

            var half = length >> 1;
            for (var i = 0; i < half; i++)
            {
                temp[i] = sourceList[i * 2 + start] / 2 + sourceList[i * 2 + 1 + start] / 2;
                temp[i + half] = sourceList[i * 2 + start] / 2 - sourceList[i * 2 + 1 + start] / 2;
            }

            for (var i = 0; i < length; i++)
                sourceList[i + start] = temp[i];
        }

        public void TransformHaarArrayInverse(RGB[] sourceList, int start, int length)
        {
            var temp = new RGB[length];

            var half = length >> 1;
            for (var i = 0; i < half; i++)
            {
                temp[i * 2] = sourceList[i + start] + sourceList[i + half + start];
                temp[i * 2 + 1] = sourceList[i + start] - sourceList[i + half + start];
            }

            for (var i = 0; i < length; i++)
                sourceList[i + start] = temp[i];
        }
    }
}
