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

        public override string ToString() => $"{ToChar(R)}{ToChar(G)}{ToChar(B)}";

        private byte ToByte(double x) => (byte)Math.Max(0, Math.Min(byte.MaxValue, x));
        private char ToChar(double x) => Convert.ToChar(ToByte(x));
        
        public static RGB operator +(RGB a, RGB b) => new(a.R + b.R, a.G + b.G, a.B + b.B);
        public static RGB operator +(RGB a, double x) => new(a.R + x, a.G + x, a.B + x);
        public static RGB operator -(RGB a, RGB b) => new(a.R - b.R, a.G - b.G, a.B - b.B);
        public static RGB operator -(RGB code, double x) => code + -x;
        public static RGB operator *(RGB code, double x) => new(code.R * x, code.G * x, code.B * x);
        public static RGB operator /(RGB code, double x) => new(code.R / x, code.G / x, code.B / x);
        public RGB Div(int x) => new(Math.Floor(R / x), Math.Floor(G / x), Math.Floor(B / x));
    }
}
