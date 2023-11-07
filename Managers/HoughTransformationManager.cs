using System;
using System.Drawing;

namespace ImageProcessingWeb.Managers
{
    public class HoughTransformationManager
    {
        private readonly ColorManager _colorManager = new();

        public Bitmap DetectLines(Bitmap source, double threshold, string lineColor)
        {
            var pixels = _colorManager.GetBitmapPixels(source);
            var width = source.Width;
            var height = source.Height;

            var diagonal = (int)Math.Sqrt(width * width + height * height);

            var maxDegree = 180;
            var accumulator = new int[maxDegree, diagonal];

            // Perform Hough Transform
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var cx = x - width / 2; // -w/2 ... w/2
                    var cy = y - height / 2; // -h/2 ... h/2
                    if (pixels[x + y * width].GetBrightness() > 0.5) continue;
                    for (var degree = 0; degree < maxDegree; degree++)
                    {  
                        // -d/2 ... d/2 
                        var rho = cx * Math.Cos(DegreesToRadians(degree)) + cy * Math.Sin(DegreesToRadians(degree)) + diagonal / 2;
                        rho = Math.Max(0, Math.Min(rho, diagonal - 1));
                        accumulator[degree, (int)rho]++;
                    }
                }

            var output = _colorManager.DrawFromPixels(pixels, width);
            var color = (Color) new ColorConverter().ConvertFromString(lineColor);
            var pen = new Pen(color);
            for (var i = 0; i < maxDegree; i++)
                for (var j = 0; j < diagonal; j++)
                {
                    if (accumulator[i, j] < threshold) continue;
                    var rho = j - diagonal / 2;
                    var graphics = Graphics.FromImage(output);
                    var a = Math.Cos(DegreesToRadians(i));
                    var b = Math.Sin(DegreesToRadians(i));
                    var x1 = a * rho - diagonal / 2 * b + width / 2;
                    var y1 = b * rho + diagonal / 2 * a + height / 2;
                    var x2 = a * rho + diagonal / 2 * b + width / 2;
                    var y2 = b * rho - diagonal / 2 * a + height / 2;
                    graphics.DrawLine(pen, (int)x1, (int)y1, (int)x2, (int)y2);
                }

            return output;
        }


        private double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
