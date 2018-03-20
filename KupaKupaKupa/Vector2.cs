using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KupaKupaKupa
{
    class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        public Vector2(Vector2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        public Vector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static Vector2 operator + (Vector2 l, Vector2 r)
        {
            return new Vector2
            {
                X = l.X + r.X,
                Y = l.Y + r.Y
            };
        }

        public static Vector2 operator * (float alpha, Vector2 r)
        {
            return new Vector2
            {
                X = r.X * alpha,
                Y = r.Y * alpha
            };
        }

        public static float operator * (Vector2 l, Vector2 r)
        {
            return l.X * r.X + l.Y * r.Y;
        }

        public static Vector2 operator - (Vector2 l, Vector2 r)
        {
            return l + (-1.0f * r);
        }

        public static Vector2 operator - (Vector2 v)
        {
            return (-1) * v;
        }

        public Vector2 rotate(float theta)
        {
            double dTheta = (double)theta;
            double nX = Math.Cos(dTheta) * X - Math.Sin(dTheta) * Y;
            double nY = Math.Cos(dTheta) * Y + Math.Sin(dTheta) * X;

            return new Vector2
            {
                X = (float)nX,
                Y = (float)nY
            };
        }

        public static Vector2 operator / (Vector2 v, float alpha)
        {
            return (1.0f / alpha) * v;
        }
        public static Vector2 rotate(float theta, Vector2 v)
        {
            return v.rotate(theta);
        }

        public float length()
        {
            return (float)Math.Sqrt(X*X+Y*Y);
        }

    }
}
