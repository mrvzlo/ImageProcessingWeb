using System.Drawing;

namespace ImageProcessingWeb.Models
{
    public class CompressionResult
    {
        public string SourceStr;
        public string ResultStr;
        public string Result;
        public string Compressed;

        public CompressionResult(Bitmap source, Bitmap output, string result, string compressed)
        {
            ResultStr = Images.BitmapToString(output);
            SourceStr = Images.BitmapToString(source);
            Result = result;
            Compressed = compressed;
        }
    }
}
