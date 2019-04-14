using System;
using System.Numerics;

namespace GameModel
{
    public class Vector
    {
        public float X;

        public float Y;

        public float Z;

        public Vector(): this(0)
        {
        }
        
        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vector(float x) : this(x, x, x)
        {
        }

        public float dotProduct(Vector v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y,a.Z + b.Z);
        }
        
        public static Vector operator +(Vector a, float x)
        {
            return new Vector(a.X + x, a.Y + x,a.Z + x);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y,a.Z - b.Z);
        }
        
        public static Vector operator -(Vector a, float x)
        {
            return new Vector(a.X - x, a.Y - x,a.Z - x);
        }
        
        public static Vector operator *(Vector a, float x)
        {
            return new Vector(a.X * x, a.Y * x,a.Z * x);
        }
        
        public static Vector operator /(Vector a, float x)
        {
            return new Vector(a.X / x, a.Y / x,a.Z / x);
        }

        public static Vector Random()
        {
            var rand = new Random();
            return new Vector(
                (float) rand.NextDouble(),
                (float) rand.NextDouble(),
                (float) rand.NextDouble());
        }

        public float Length()
        {
            return (float) Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        
        public Vector Normalize()
        {
            var l = Length();
            return new Vector(X / l, Y / l, Z / l);
        }

        public float Distance(Vector b)
        {
            return Math.Abs((b - this).Length());
        }
    }
}