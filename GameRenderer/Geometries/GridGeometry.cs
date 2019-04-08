using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class GridGeometry : ColorVertexGeometry
    {
        private float size = 100.0f;

        private float cellSize = 1.0f;

        private  vec4 color;

        public GridGeometry(float size, float cellSize, vec4 color)
        {
            this.size = size;
            this.cellSize = cellSize;
            this.color = color;

            Mode = PrimitiveType.Lines;

            UpdateData();
        }
        
        private void UpdateData()
        {
            var data = new List<ColorVertex>();
            
            for (float i = -size; i <  size; i+=cellSize) {
                data.Add(new ColorVertex(new vec3(i, 0.0f, -size), color));
                data.Add(new ColorVertex(new vec3(i, 0.0f, size), color));
            }

            for (float i = -size; i <  size; i+=cellSize) {
                data.Add(new ColorVertex(new vec3(-size, 0.0f, i), color));
                data.Add(new ColorVertex(new vec3(size, 0.0f, i), color));
            }
            UpdateData(data.ToRawArray());
        }
    }
}