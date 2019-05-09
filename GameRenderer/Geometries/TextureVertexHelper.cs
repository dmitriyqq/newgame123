using System.Collections.Generic;

namespace GameRenderer
{
    public static class TextureVertexHelper
    {
        public static float[] ToRawArray(this List<TextureVertex> l)
        {
            float[] data = new float[l.Count * TextureVertex.FloatSize];

            for (int i = 0; i < l.Count; i++)
            {
                var index = i * TextureVertex.FloatSize;
                data[index + 0] = l[i].Position.x;
                data[index + 1] = l[i].Position.y;
                data[index + 2] = l[i].Position.z;

                data[index + 3] = l[i].Normal.x;
                data[index + 4] = l[i].Normal.y;
                data[index + 5] = l[i].Normal.z;
                
                data[index + 6] = l[i].Texture.x;
                data[index + 7] = l[i].Texture.y;
            }

            return data;
        }
    }
}