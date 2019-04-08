using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class LineGeometry : ColorVertexGeometry
    {
        private vec4 color;
        
        public LineGeometry(vec3 start, vec3 end, vec4 color)
        {
            this.color = color;
            Mode = PrimitiveType.Lines;
            Update(start, end);
        }
        
        public void Update(vec3 start, vec3 end)
        {
            UpdateData(new List<ColorVertex>
            {
                new ColorVertex(start, color),
                new ColorVertex(end, color),
            }.ToRawArray());
        }
    }
}