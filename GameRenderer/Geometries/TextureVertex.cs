using GlmNet;

namespace GameRenderer
{
    public class TextureVertex
    {
        public vec3 Position;

        public vec3 Normal;

        public vec2 Texture;

        public static int FloatSize => 8;
        
        public static int ByteSize => FloatSize * sizeof(float);

        public TextureVertex(float x, float y, float z, float n1, float n2, float n3, float u, float v)
        {
            Position = new vec3(x, y, z);
            Normal = new vec3(n1, n2, n3);
            Texture = new vec2(u, v);
        }
    }
}