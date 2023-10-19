using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Console;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImageProcessingWeb.Pages
{
    public class IndexModel : PageModel
    {
        public string SourceImage { get; set; }
        public string SourceHistogram { get; set; }
        public string OutputImage { get; set; }
        public string OutputHistogram { get; set; }

        private FourierFilterManager _fourierFilterManager = new();
        private GaussFilterManager _gaussFilterManager = new();
        private HistogramManager _histogramManager = new();


        public void OnGet() { }

        public void OnPost(string action, int sigma = 10)
        {
            var imageFile = Request.Form.Files["image"];
            if (imageFile == null) return;
            using var memoryStream = new MemoryStream();
            imageFile.CopyTo(memoryStream);
            var input = new Bitmap(memoryStream);
            Bitmap output = null;
            Bitmap ih = null;
            Bitmap oh = null;
            switch (action)
            {
                case "gauss":
                    ih = _histogramManager.GenerateHistogramImage(input);
                    output = _gaussFilterManager.ApplyFilter(input, sigma);
                    oh = _histogramManager.GenerateHistogramImage(output);
                    break;
                case "equalize":
                    ih = _histogramManager.GenerateHistogramImage(input);
                    output = _histogramManager.NormalizeHistogram(input);
                    oh = _histogramManager.GenerateHistogramImage(output);
                    break;
            }

            SourceHistogram = BitmapToString(ih);
            SourceImage = BitmapToString(input);
            OutputHistogram = BitmapToString(oh);
            OutputImage = BitmapToString(output);
        }

        private string BitmapToString(Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            var imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}
