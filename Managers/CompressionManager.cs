using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ImageProcessingWeb.Models;
using ImageProcessingWeb.Pages;

namespace ImageProcessingWeb.Managers
{
    public class CompressionManager
    {
        private readonly ColorManager _colorManager = new();

        public CompressionResult Compress(Bitmap source)
        {
            var pixels = _colorManager.GetBitmapPixels(source).Select(x => new RGB(x)).ToArray();
            var size = pixels.Length * 3;
            var compressed = Compress(PixelsToString(Predict(pixels, source.Width)));
            var decompressed = Decompress(compressed);

            var defaultCompression = Compress(PixelsToString(pixels)).Length;
            var defaultRate = 100 - Math.Round(100d * defaultCompression / size);

            var predictionRate = 100 - Math.Round(100d * compressed.Length / size);
            var quality = 100 - Math.Round(100d * defaultRate / predictionRate);
            var text = $"Source size: {size}\n " +
                       $"Compression rate without predictions {defaultRate}% to {defaultCompression}\n" +
                       $"Compression rate with predictions {predictionRate}% to {compressed.Length}\n" +
                       $"Difference is {quality}%";
            return new CompressionResult(source, RestoreBitmap(decompressed, source.Width), text, compressed);
        }

        private string PixelsToString(RGB[] pixels) => string.Join("", pixels.Select(x => x.ToString()));

        private Bitmap RestoreBitmap(string str, int width)
        {
            var pixels = DecompressToPixels(str);
            var decompressedPixels = RestorePrediction(pixels, width);
            var restored = decompressedPixels.Select(x => x.ToColor()).ToList();
            return _colorManager.DrawFromPixels(restored, width);
        }

        private RGB[] DecompressToPixels(string pixels)
        {
            var decompressed = Decompress(pixels);
            if (decompressed.Length % 3 != 0) throw new Exception("Cannot be decompressed");
            var result = new RGB[decompressed.Length / 3];
            for (var i = 0; i < decompressed.Length; i += 3)
            {
                var r = (byte)decompressed[i];
                var g = (byte)decompressed[i + 1];
                var b = (byte)decompressed[i + 2];
                result[i / 3] = new RGB(r, g, b);
            }

            return result;
        }

        private RGB[] Predict(RGB[] pixels, int width)
        {
            pixels = pixels.Select(x => x.Div(4) * 2).ToArray();
            var result = new RGB[pixels.Length];

            for (var i = 0; i < pixels.Length; i++)
            {
                if (i % width == 0 || i < width)
                {
                    result[i] = pixels[i];
                    continue;
                }
                result[i] = pixels[i] - pixels[i - 1] / 2 - pixels[i - width] / 2 + 128;
            }

            return result;
        }

        private RGB[] RestorePrediction(RGB[] pixels, int width)
        {
            var restored = new RGB[pixels.Length];
            for (var i = 0; i < pixels.Length; i++)
            {
                if (i % width == 0 || i < width)
                {
                    restored[i] = pixels[i];
                    continue;
                }
                restored[i] = restored[i - 1] / 2 + restored[i - width] / 2 + pixels[i] - 128;
            }

            return restored.Select(x => x * 2).ToArray();
        }

        private string Compress(string input)
        {
            var dictionary = new Dictionary<string, char>();
            var code = 0;
            for (; code < 256; code++)
                dictionary[((char)code).ToString()] = (char)code;

            var result = "";
            var current = "";

            foreach (var symbol in input)
            {
                var next = current + symbol;
                if (dictionary.ContainsKey(next))
                {
                    current = next;
                    continue;
                }

                result += dictionary[current];
                dictionary[next] = (char)code;
                code++;
                current = symbol.ToString();
            }

            if (!string.IsNullOrEmpty(current))
                result += dictionary[current];

            return result;
        }

        private string Decompress(string compressed)
        {
            var dictionary = new Dictionary<int, string>();
            var code = 0;
            for (; code < 256; code++)
                dictionary[code] = ((char)code).ToString();

            var previous = dictionary[compressed[0]];
            compressed = compressed.Remove(0, 1);
            var result = previous;

            foreach (var currentCode in compressed)
            {
                var current = "";
                if (dictionary.ContainsKey(currentCode))
                    current = dictionary[currentCode];
                else if (currentCode == code)
                    current = previous + previous[0];

                result += current;
                dictionary[code] = previous + current[0];
                code++;
                previous = current;
            }

            return result;
        }
    }
}
