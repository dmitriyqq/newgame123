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
        
        public static vec3 Polar2Cartesian (float radius, float theta, float alpha)
        {
            return new vec3(
                radius * (float) Math.Sin(theta) * (float) Math.Cos(alpha),
                radius * (float) Math.Sin(theta) * (float) Math.Sin(alpha),
                radius * (float) Math.Cos(theta)
            );
        }

        public static Vector3 ToVector3(this vec3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static vec3 ToGlm(this Vector3D v)
        {
            return new vec3(v.X, v.Y, v.Z);
        }

        public static vec4 ToGlm(this Color4D v)
        {
            return new vec4(v.R, v.G, v.B, v.A);
        }

        public static mat4 GetModelMatrix(vec3 position, vec3 rotation, vec3 scale)
        {
            var c = glm.lookAt(position, position + rotation, new vec3(0.0f, 1.0f, 0.0f));
            return c * glm.scale(new mat4(1), scale);
        }

        public static vec3 Normal(vec3 a, vec3 b, vec3 c)
        {
            var dir = glm.cross(b - a, c - a);
            var norm = glm.normalize(dir);
            return norm;
        }
    }
}