using System;
using System.Collections.Generic;
using GameModel;
using GameModel.GameObjects;
using GameRenderer.Materials;
using GameRenderer.Metadata;
using GameRenderer.Metadata.Assets;
using GameRenderer.OpenGL;
using GlmNet;
using ModelLoader;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class MapDrawable : CompoundMesh
    {
        private readonly Map _map;
        private readonly Texture _texture;
        private readonly Material _material;
        
        private readonly VertexArray _geometry;

        private readonly Buffer<float> _positions;
        private readonly Buffer<float> _textures;
        private readonly Buffer<float> _normals;
        
        private readonly Logger _logger;
        private readonly MaterialManager _materialManager;

        public MapDrawable(MapAsset asset, Map map, Logger logger, MaterialManager manager)
        {
            _map = map;
            _logger = logger;
            _materialManager = manager;
            _geometry = new VertexArray(_map.Size * _map.Size, PrimitiveType.Triangles, _logger);

            var (positions, normals, textures) = GetUpdatedData();
            _positions = _geometry.AttachComponent(Constants.PositionsComponent, BufferUsageHint.DynamicDraw, positions, 3, 0);
            _normals = _geometry.AttachComponent(Constants.PositionsComponent, BufferUsageHint.DynamicDraw, normals, 3, 0);
            _textures = _geometry.AttachComponent(Constants.PositionsComponent, BufferUsageHint.DynamicDraw, textures, 2, 0);
            _geometry.UseIndices(GetIndices());
            _geometry.GenerateVertexAttribPointer();

            AddChild(CreateSkybox(asset.SkyboxAsset));
            
            var mapTexture = new Texture(asset.DiffuseTexture);
            var mapTexture2 = new Texture(asset.SpecularTexture);
            var material = _materialManager.GetMaterial(asset.MaterialName);
            material.Textures[0] = mapTexture;
            material.Textures[1] = mapTexture2;
            
            Geometry = _geometry;
            Material = material;
            
            map.OnUpdate += GenerateMap;
            GenerateMap();

            foreach (var uniformValue in asset.MaterialParameters)
            {
                Material.Uniform(uniformValue.Uniform, uniformValue.Value);
            }
        }

        private SkyboxMesh CreateSkybox(SkyboxAsset asset)
        {
            var geometry = new CubeGeometry();
            var material = _materialManager.GetMaterial(asset.MaterialName);

            material.Textures[0] = new SkyboxTexture(new List<string>
            {
                asset.Front,
                asset.Back,
                asset.Top,
                asset.Bottom,
                asset.Right,
                asset.Left
            });

            var mesh = new SkyboxMesh(geometry, material) {Parent = this};
            return mesh;
        }

        private void GenerateMap()
        {
            var (positions, normals, textures) = GetUpdatedData();
            _positions.BufferData(positions);
            _normals.BufferData(normals);
            _textures.BufferData(textures);
        }

        private (float[], float[], float[]) GetUpdatedData()
        {
            var positions = GetPositions();
            var normals = GetNormals(positions);
            var textures = GetTextures();
            return (positions.ToRawArray(), normals.ToRawArray(), textures.ToRawArray());
        }

        private vec3[] GetPositions()
        {
            var positions = new vec3[_map.Size * _map.Size];
            var index = 0;

            TraverseVertices((i, j, p) =>
            {
                positions[index] = p;
                index++;
            });

            return positions;
        }
        
        private vec3[] GetNormals(vec3[] positions)
        {
            var normals = new vec3[_map.Size * _map.Size];
            TraverseVoxels((v00, v01, v10, v11, triangleIndex) =>
            {
                var pv00 = positions[v00]; 
                var pv01 = positions[v01];
                var pv10 = positions[v10];
                
                var n1 = VectorHelper.Normal(pv00, pv01, pv10);

                normals[v00] = n1;
                normals[v01] = n1;
                normals[v10] = n1;
                normals[v11] = n1;
            });

            return normals;
        }

        private vec2[] GetTextures()
        {
            var uv = new vec2[_map.Size * _map.Size];
            var trUv = new vec2(0.0f, 1.0f);
            var tlUv = new vec2(1.0f, 1.0f);
            var brUv = new vec2(0.0f, 0.0f);
            var blUv = new vec2(1.0f, 0.0f);

            TraverseVoxels((v00, v01, v10, v11, triangleIndex) =>
            {
                uv[v00] = tlUv;
                uv[v01] = trUv;
                uv[v10] = blUv;
                uv[v11] = brUv;
            });

            return uv;
        }

        private int[] GetIndices()
        {
            var width = _map.Size;
            var height = _map.Size;
            var quadWidth = width - 1;
            var quadHeight = height - 1;
            var triangleCount = quadWidth * quadHeight * 2;
            var indices = new int[triangleCount * 3];

            TraverseVoxels((v00, v01, v10, v11, triangleIndex) =>
            {
                indices[3 * triangleIndex] = v00;
                indices[3 * triangleIndex + 1] = v01;
                indices[3 * triangleIndex + 2] = v10;
                    
                indices[3 * triangleIndex + 3] = v01;
                indices[3 * triangleIndex + 4] = v11;
                indices[3 * triangleIndex + 5] = v10;
            });

            return indices;
        }

        private void TraverseVertices(Action<int, int, vec3> func)
        {
            var width = _map.Size;
            var height = _map.Size;

            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    func(i, j, GetVertexInPosition(i, j));
                }
            }
        }

        private void TraverseVoxels(Action<int, int, int, int, int> func)
        {
            var width = _map.Size;
            var height = _map.Size;
            var quadWidth = width - 1;
            var quadHeight = height - 1;
            for (var i = 0; i < quadWidth; ++i)
            {
                for (var j = 0; j < quadHeight; ++j)
                {
                    var triangleIndex = (j * quadWidth + i) * 2;
                    var v00 = width * j + i;
                    var v01 = width * j + i + 1;
                    var v10 = width * (j + 1) + i;
                    var v11 = width * (j + 1) + i + 1;
                    func(v00, v01, v10, v11, triangleIndex);
                }
            }
        }

        private vec3 GetVertexInPosition(int i, int j)
        {
            var y = _map.Data[i, j];
            var x = (i - _map.Size / 2) * _map.Resolution;
            var z = -(j - _map.Size / 2) * _map.Resolution;

            return new vec3(z, y, x);
        }
    }
}