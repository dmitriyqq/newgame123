using System;
using System.Numerics;
using Assimp;
using GlmNet;
using OpenTK.Graphics.OpenGL;

namespace GameModel
{
    public static class VectorHelper
    {
        public static vec3 ToGlm(this Vector3 v)
        {
            return new vec3(v.X, v.Y, v.Z);
        }
        
        public static vec3 ToGlm(this Vector v)
        {
            return new vec3(v.X, v.Y, v.Z);
        }
        
        public static vec3 Polar2Cartesian (float radius, float theta, float alpha)
        {
            return new vec3(
                radius * (float) Math.Sin(theta) * (float) Math.Cos(alpha),
                radius * (float) Math.Sin(theta) * (float) Math.Sin(alpha),
                radius * (float) Math.Cos(theta)
            );
        }

        public static Vector ToVector(this vec3 v)
        {
            return new Vector(v.x, v.y, v.z);
        }

        public static vec3 ToGlm(this Vector3D v)
        {
            return new vec3(v.X, v.Y, v.Z);
        }
        
        public static vec4 ToGlm(this Color4D v)
        {
            return new vec4(v.R, v.G, v.B, v.A);
        }
    }
}