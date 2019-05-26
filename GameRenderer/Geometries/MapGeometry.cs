using System.Collections.Generic;
using GameModel;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class MapGeometry : IndexedTextureGeometry
    {
        private Map map;

        private vec4 color = new vec4(1.0f, 1.0f, 0.0f, 1.0f);
        
        public MapGeometry(Map map)
        {
            this.map = map;

            map.OnUpdate += GenerateMap;
            Mode = PrimitiveType.Triangles;

            GenerateMap();
        }

        public void GenerateMap()
        {
            ConstructData();
        }

        private vec3 GetVertexInPosition(int i, int j)
        {
            float y = map.data[i, j];
            float x = (i - map.Size / 2) * map.Resolution;
            float z = -(j - map.Size / 2) * map.Resolution;

            return new vec3(z, y, x);
        }

        public void ConstructData()
        {
            var vertices = new TextureVertex[map.Size * map.Size];
            var width = map.Size;
            var height = map.Size;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    vertices[width * j + i] = new TextureVertex(GetVertexInPosition(i, j));
                }
            }

            var quadWidth = width - 1;
            var quadHeight = height - 1;
            var triangleCount = quadWidth * quadHeight * 2;

            var trUv = new vec2(0.0f, 1.0f);
            var tlUv = new vec2(1.0f, 1.0f);
            var brUv = new vec2(0.0f, 0.0f);
            var blUv = new vec2(1.0f, 0.0f);
            
            var indicies = new int[triangleCount * 3];
            for (int i = 0; i < quadWidth; ++i)
            {
                for (int j = 0; j < quadHeight; ++j)
                {
                    var triangleIndex = (j * quadWidth + i) * 2;
                    var v00 = width * j + i;
                    var v01 = width * j + i + 1;
                    var v10 = width * (j + 1) + i;
                    var v11 = width * (j + 1) + i + 1;

                    var vv00 = vertices[v00];
                    var vv01 = vertices[v01];
                    var vv10 = vertices[v10];
                    var vv11 = vertices[v11];

                    var n1 = VectorHelper.Normal(vv00.Position, vv01.Position, vv10.Position);
//                    var n2 = -1 * VectorHelper.Normal(vv01.Position, vv10.Position, vv11.Position);
                    
                    vv00.Texture = tlUv;
                    vv01.Texture = trUv;
                    vv10.Texture = blUv;
                    vv11.Texture = brUv;
                    
                    vv00.Normal = n1;
                    vv01.Normal = n1;
                    vv10.Normal = n1;
                    vv11.Normal = n1;
                    
                    indicies[3 * triangleIndex] = v00;
                    indicies[3 * triangleIndex + 1] = v01;
                    indicies[3 * triangleIndex + 2] = v10;
                    
                    indicies[3 * triangleIndex + 3] = v01;
                    indicies[3 * triangleIndex + 4] = v11;
                    indicies[3 * triangleIndex + 5] = v10;
                }
            }

            UpdateData(vertices.ToRawArray());
            UpdateIndicies(indicies);
        }
    }
}