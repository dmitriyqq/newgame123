using System.Collections.Generic;
using GameModel;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class MapGeometry : TextureVertexGeometry
    {
        private Map map;

        private vec4 color = new vec4(1.0f, 1.0f, 0.0f, 1.0f);
        
        public MapGeometry(Map map)
        {
            this.map = map;

            Mode = PrimitiveType.Triangles;

            GenerateMap();
        }

        public void GenerateMap()
        {
            UpdateData(ConstructData());
        }

        private vec3 getVertexInPosition(int i, int j)
        {
            float z = map.data[i, j];
            float x = (i - map.Size / 2) * map.Resolution;
            float y = (j - map.Size / 2) * map.Resolution;

            return new vec3(x, z, y);
        }

        public float[] ConstructData()
        {
            List<TextureVertex> vertices = new List<TextureVertex>();
            for (int i = 0; i < map.Size - 1; i++)
            {
                for (int j = 0; j < map.Size - 1; j++)
                {
                    var p1 = getVertexInPosition(i, j);
                    var p2 = getVertexInPosition(i+1, j);
                    var p3 = getVertexInPosition(i, j+1);
                    var p4 = getVertexInPosition(i+1, j+1);

                    var n1 = VectorHelper.Normal(p1, p2, p3);
                    var n2 = -1 * VectorHelper.Normal(p2, p3, p4);
                    
                    var trUv = new vec2(0.0f, 1.0f);
                    var tlUv = new vec2(1.0f, 1.0f);
                    var brUv = new vec2(0.0f, 0.0f);
                    var blUv = new vec2(1.0f, 0.0f);

                    vertices.Add(new TextureVertex(p1, n1, tlUv));
                    vertices.Add(new TextureVertex(p2, n1, trUv));
                    vertices.Add(new TextureVertex(p3, n1, blUv));
                    vertices.Add(new TextureVertex(p2, n2, trUv));
                    vertices.Add(new TextureVertex(p3, n2, blUv));
                    vertices.Add(new TextureVertex(p4, n2, brUv));
                }
            }

            return vertices.ToRawArray();
        }
    }
}