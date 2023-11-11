using System.Drawing;
using System.IO;
using Console;
using ImageProcessingWeb.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImageProcessingWeb.Pages
{
    public class IndexModel : PageModel
    {
        private FourierFilterManager _fourierFilterManager = new();
        private GaussFilterManager _gaussFilterManager = new();
        private HistogramManager _histogramManager = new();
        private HueRotationManager _hueRotationManager = new();
        private BicubicUpscaleManager _bicubicUpscaleManager = new();
        private HaarNoiseReductionManager _haarNoiseReductionManager = new();
        private HoughTransformationManager _houghTransformationManager = new();
        private CompressionManager _compressionManager = new();

        public void OnGet() { }

        public PartialViewResult OnPost(string action, int sigma, int degree, int threshold, int hough, string lineColor)
        {
            var imageFile = Request.Form.Files["image"];
            if (imageFile == null) return Partial("_Result", new Images());
            using var memoryStream = new MemoryStream();
            imageFile.CopyTo(memoryStream);
            var result = new Images {Source = new Bitmap(memoryStream)};

            if (action == "compress")
            {
                var compressionResult = _compressionManager.Compress(result.Source);
                return Partial("_CompressionResult", compressionResult);
            }

            result.SourceHistogram = _histogramManager.GenerateHistogramImage(result.Source);
            result.Output = action switch
            {
                "gauss" => _gaussFilterManager.ApplyFilter(result.Source, sigma),
                "equalize" => _histogramManager.NormalizeHistogram(result.Source),
                "rotate" => _hueRotationManager.Rotate(result.Source, degree),
                "bicubic" => _bicubicUpscaleManager.Upscale(result.Source),
                "denoise" => _haarNoiseReductionManager.Denoise(result.Source, threshold),
                "hough" => _houghTransformationManager.DetectLines(result.Source, hough, lineColor),
                _ => result.Source
            };
            result.OutputHistogram = _histogramManager.GenerateHistogramImage(result.Output);
            result.Stringify();
            result.ShowHistogram = action == "equalize";
            return Partial("_Result", result);
        }
    }
}
