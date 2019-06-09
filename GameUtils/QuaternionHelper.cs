using System;
using System.Numerics;
using GlmNet;

namespace GameUtils
{
    public static class QuaternionHelper
    {
        public static mat4 ToRotationMatrix(this Quaternion q) {
            var matrix = mat4.identity();
            var xy = q.X * q.Y;
            var xz = q.X * q.Z;
            var xw = q.X * q.W;
            var yz = q.Y * q.Z;
            var yw = q.Y * q.W;
            var zw = q.Z * q.W;
            var xSquared = q.X * q.X;
            var ySquared = q.Y * q.Y;
            var zSquared = q.Z * q.Z;
            matrix[0, 0] = 1 - 2 * (ySquared + zSquared);
            matrix[0, 1] = 2 * (xy - zw);
            matrix[0, 2] = 2 * (xz + yw);
            matrix[0, 3] = 0;
            matrix[1, 0] = 2 * (xy + zw);
            matrix[1, 1] = 1 - 2 * (xSquared + zSquared);
            matrix[1, 2] = 2 * (yz - xw);
            matrix[1, 3] = 0;
            matrix[2, 0] = 2 * (xz - yw);
            matrix[2, 1] = 2 * (yz + xw);
            matrix[2, 2] = 1 - 2 * (xSquared + ySquared);
            matrix[2, 3] = 0;
            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;
            return matrix;
        }
        
        public static Quaternion FromMatrix(mat4 matrix) {
            float w, x, y, z;
            var diagonal = matrix[0, 0] + matrix[1, 1] + matrix[2, 2];

            if (diagonal > 0) {
                var w4 = (float) (Math.Sqrt(diagonal + 1f) * 2f);
                w = w4 / 4.0f;
                x = (matrix[2, 1] - matrix[1, 2]) / w4;
                y = (matrix[0, 2] - matrix[2, 0]) / w4;
                z = (matrix[1, 0] - matrix[0, 1]) / w4;
            } else if ((matrix[0, 0] > matrix[1,1]) && (matrix[0, 0] > matrix[2, 2])) {
                var x4 = (float) (Math.Sqrt(1f + matrix[0, 0] - matrix[1, 1] - matrix[2, 2]) * 2f);
                w = (matrix[2, 1] - matrix[1, 2]) / x4;
                y = (matrix[0, 1] + matrix[1, 0]) / x4;
                z = (matrix[0, 2] + matrix[2, 0]) / x4;
                x = x4 / 4.0f;
            } else if (matrix[1, 1] > matrix[2, 2]) {
                var y4 = (float) (Math.Sqrt(1f + matrix[1, 1] - matrix[0, 0] - matrix[2, 2]) * 2f);
                w = (matrix[0, 2] - matrix[2, 0]) / y4;
                x = (matrix[0, 1] + matrix[1, 0]) / y4;
                z = (matrix[1, 2] + matrix[2, 1]) / y4;
                y = y4 / 4.0f;
            } else {
                var z4 = (float) (Math.Sqrt(1f + matrix[2, 2] - matrix[0, 0] - matrix[1, 1]) * 2f);
                w = (matrix[1, 0] - matrix[0, 1]) / z4;
                x = (matrix[0, 2] + matrix[2, 0]) / z4;
                y = (matrix[1, 2] + matrix[2, 1]) / z4;
                z = z4 / 4.0f;
            }
            return new Quaternion(x, y, z, w);
        }
    }
}