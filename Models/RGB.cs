using System;
using System.Drawing;

namespace ImageProcessingWeb.Models
{
    public class RGB
    {
        public double R;
        public double G;
        public double B;

        public RGB() { }

        public RGB(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public RGB(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color ToColor() => Color.FromArgb(ToByte(R), ToByte(G), ToByte(B));

        private byte ToByte(double x) => (byte)Math.Max(0, Math.Min(byte.MaxValue, x));

        public static RGB operator +(RGB code, double x)
        {
            code.R += x;
            code.G += x;
            code.B += x;
            return code;
        }

        public static RGB operator +(RGB a, RGB b) => new() {R = a.R + b.R, G = a.G + b.G, B = a.B + b.B};

        public static RGB operator -(RGB a, RGB b) => new() { R = a.R - b.R, G = a.G - b.G, B = a.B - b.B };

        public static RGB operator -(RGB code, double x) => code + -x;

        public static RGB operator *(RGB code, double x)
        {
            code.R *= x;
            code.G *= x;
            code.B *= x;
            return code;
        }

        public static RGB operator /(RGB code, double x)
        {
            code.R /= x;
            code.G /= x;
            code.B /= x;
            return code;
        }
    }
}
