using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessingWeb
{
    public class Images
    {
        public Bitmap Source;
        public Bitmap SourceHistogram;
        public Bitmap Output;
        public Bitmap OutputHistogram;

        public bool ShowHistogram = false;

        public string SourceStr;
        public string SourceHistogramStr;
        public string OutputStr;
        public string OutputHistogramStr;

        public void Stringify()
        {
            SourceStr = BitmapToString(Source);
            SourceHistogramStr = BitmapToString(SourceHistogram);
            OutputStr = BitmapToString(Output);
            OutputHistogramStr = BitmapToString(OutputHistogram);
        }

        private string BitmapToString(Bitmap bitmap)
        {
            if (bitmap == null) return "";
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            var imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }
}
