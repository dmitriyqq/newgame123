using System.Collections.Generic;

namespace GameRenderer
{
    public static class ColorVertexHelper
    {
        public static float[] ToRawArray(this List<ColorVertex> l)
        {
            float[] data = new float[l.Count * ColorVertex.FloatSize];

            for (int i = 0; i < l.Count; i++)
            {
                var index = i * ColorVertex.FloatSize;
                data[index + 0] = l[i].Position.x;
                data[index + 1] = l[i].Position.y;
                data[index + 2] = l[i].Position.z;

                data[index + 3] = l[i].Color.x;
                data[index + 4] = l[i].Color.y;
                data[index + 5] = l[i].Color.z;
                data[index + 6] = l[i].Color.w;
            }

            return data;
        }
    }
}