using System;
using System.Drawing;
using System.Numerics;

namespace ImageProcessingWeb.Models
{
    public class ComplexRGB
    {
        public Complex R;
        public Complex G;
        public Complex B;

        public ComplexRGB(double r, double g, double b)
        {
            R = new Complex(r, 0);
            G = new Complex(g, 0);
            B = new Complex(b, 0);
        }

        public ComplexRGB(Complex r, Complex g, Complex b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color ToColor() => new RGB(R.Magnitude, G.Magnitude, B.Magnitude).ToColor();
        
        public static ComplexRGB operator +(ComplexRGB a, ComplexRGB b) => new(a.R + b.R, a.G + b.G, a.B + b.B);
        public static ComplexRGB operator +(ComplexRGB a, double x) => new(a.R + x, a.G + x, a.B + x);
        public static ComplexRGB operator -(ComplexRGB a, ComplexRGB b) => new(a.R - b.R, a.G - b.G, a.B - b.B);
        public static ComplexRGB operator -(ComplexRGB code, double x) => code + -x;
        public static ComplexRGB operator *(ComplexRGB code, double x) => new(code.R * x, code.G * x, code.B * x);
        public static ComplexRGB operator *(ComplexRGB code, Complex x) => new(code.R * x, code.G * x, code.B * x);
        public static ComplexRGB operator /(ComplexRGB code, double x) => new(code.R / x, code.G / x, code.B / x);
    }
}
