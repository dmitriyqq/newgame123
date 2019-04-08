using GlmNet;

namespace GameRenderer
{
    public class ColorVertex
    {
        public vec3 Position;

        public vec4 Color;

        public static int FloatSize => 7;

        public static int ByteSize  => FloatSize * sizeof(float);
        
        public ColorVertex(vec3 pos, vec4 color)
        {
            Position = pos;
            Color = color;
        }
    }
}