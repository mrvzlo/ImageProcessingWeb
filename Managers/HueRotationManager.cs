using System.Drawing;
using System.Linq;

namespace ImageProcessingWeb.Managers
{
    public class HueRotationManager
    {
        private readonly ColorManager _colorManager = new();

        // for HSV, HSL and HSI rotation is the same because only H value changes, remaining others unchanged.
        public Bitmap Rotate(Bitmap source, int value)
        {
            var pixels = _colorManager.GetBitmapPixels(source);
            var newPixels = pixels.Select(x => 
                _colorManager.ColorFromHSV(x.GetHue() + value, x.GetSaturation(), _colorManager.GetValue(x))).ToList();
            return _colorManager.DrawFromPixels(newPixels, source.Width);
        }
    }
}
