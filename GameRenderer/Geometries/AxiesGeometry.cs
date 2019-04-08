using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class AxiesGeometry : ColorVertexGeometry
    {
        private readonly vec4 forwardColor = new vec4(1.0f, 0.0f, 0.0f, 1.0f);
        
        private readonly vec4 rightColor = new vec4(0.0f, 1.0f, 0.0f, 1.0f);
        
        private readonly vec4 upColor = new vec4(0.0f, 0.0f, 1.0f, 1.0f);

        private float size = 3.0f;
        
        public AxiesGeometry(vec3 position, vec3 forward, vec3 right, vec3 up, float size = 3.0f)
        {
            Update(position, forward, right, up);
            
            Mode = PrimitiveType.Lines;
        }
        
        public void Update(vec3 position, vec3 forward, vec3 right, vec3 up)
        {
            UpdateData(new List<ColorVertex>
            {
                new ColorVertex(position, forwardColor),
                new ColorVertex(position+ size * glm.normalize(forward), forwardColor),
                new ColorVertex(position, rightColor),
                new ColorVertex(position + size * glm.normalize(right), rightColor),
                new ColorVertex(position, upColor),
                new ColorVertex(position + size * glm.normalize(up), upColor),
            }.ToRawArray());
        }
    }
}