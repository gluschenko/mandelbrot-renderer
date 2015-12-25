using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractals
{
    struct Complex
    {
        public double Re;
        public double Im;

        public static readonly Complex Zero = new Complex(0, 0);
        public static readonly Complex I = new Complex(0, 1);
        public static readonly Complex MaxValue = new Complex(double.MaxValue, double.MaxValue);
        public static readonly Complex MinValue = new Complex(double.MinValue, double.MinValue);
        private static readonly double halfOfRoot2 = 0.5 * Math.Sqrt(2);

        public Complex(double real, double imaginary)
        {
            Re = real;
            Im = imaginary;
        }

        //

        public static Complex operator +(Complex x, Complex y)
        {
            return new Complex(x.Re + y.Re, x.Im + y.Im);
        }

        public static Complex operator +(Complex a, double f)
        {
            a.Re = (double)(a.Re + f);
            return a;
        }

        public static Complex operator +(double f, Complex a)
        {
            a.Re = (double)(a.Re + f);
            return a;
        }

        //

        public static Complex operator *(Complex x, Complex y)
        {
            return new Complex(x.Re * y.Re - x.Im * y.Im, x.Re * y.Im + x.Im * y.Re);
        }

        public static Complex operator *(Complex a, double f)
        {
            a.Re = (double)(a.Re * f);
            a.Im = (double)(a.Im * f);
            return a;
        }

        public static Complex operator *(double f, Complex a)
        {
            a.Re = (double)(a.Re * f);
            a.Im = (double)(a.Im * f);
            return a;
        }

        //

        public static Complex operator /(Complex a, double f)
        {
            if (f == 0)
            {
                throw new DivideByZeroException();
            }

            a.Re = (double)(a.Re / f);
            a.Im = (double)(a.Im / f);

            return a;
        }

        public static Complex operator /(Complex a, Complex b)
        {
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;
            double denom = u * u + v * v;

            if (denom != 0)
            {
                a.Re = (double)((x * u + y * v) / denom);
                a.Im = (double)((y * u - x * v) / denom);
            }

            /*a.Re = (double)((x * u + y * v) / denom);
            a.Im = (double)((y * u - x * v) / denom);*/

            return a;
        }

        //

        public static Complex operator -(Complex a)
        {
            a.Re = -a.Re;
            a.Im = -a.Im;
            return a;
        }

        public static Complex operator -(Complex a, double f)
        {
            a.Re = (double)(a.Re - f);
            return a;
        }

        public static Complex operator -(double f, Complex a)
        {
            a.Re = (float)(f - a.Re);
            a.Im = (float)(0 - a.Im);
            return a;
        }

        public static Complex operator -(Complex a, Complex b)
        {
            a.Re = a.Re - b.Re;
            a.Im = a.Im - b.Im;
            return a;
        }

        //

        public double Norm()
        {
            return Re * Re + Im * Im;
        }

        public double GetModulus()
        {
            double x = this.Re;
            double y = this.Im;
            return Math.Sqrt(x * x + y * y);
        }

        public static Complex Pow(Complex c, double exponent)
        {
            double x = c.Re;
            double y = c.Im;

            double modulus = Math.Pow(x * x + y * y, exponent * 0.5);
            double argument = Math.Atan2(y, x) * exponent;

            c.Re = (double)(modulus * System.Math.Cos(argument));
            c.Im = (double)(modulus * System.Math.Sin(argument));

            return c;
        }

        public static Complex Sqrt(Complex c)
        {
            double x = c.Re;
            double y = c.Im;
            double modulus = Math.Sqrt(x * x + y * y);

            int sign = (y < 0) ? -1 : 1;

            c.Re = (double)(halfOfRoot2 * Math.Sqrt(modulus + x));
            c.Im = (double)(halfOfRoot2 * sign * Math.Sqrt(modulus - x));

            return c;
        }
    }
}
