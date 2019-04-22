using System.Collections.Generic;
using Assimp;
using GameModel;
using GlmNet;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GameRenderer
{
    public class Scene : IDrawable
    {
        private List<Mesh> childs;

        public Assimp.Scene scene;
        
        public IDrawable Parent { get; set; }

        public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();

        protected Scene(List<Mesh> childs, Assimp.Scene scene)
        {
            this.childs = childs;
            this.scene = scene;

            foreach (var child in childs)
            {
                child.Parent = this;
            }
        }

        public Texture loadTexture(string filepath)
        {
            var f = $"{Directory}/{filepath}";
            if (Textures.ContainsKey(f))
            {
                return Textures[f];
            }

            var t = new Texture(f);
            Textures[f] = t;
            return t;
        }
        
        public string Name { get; set; }
        
        public string Directory { get; set; }

        public string Path => $"{Directory}/{Name}";
        
        public Scene(string directory, string name)
        {
            childs = new List<Mesh>();
            Name = name;
            Directory = directory;
            using (var context = new AssimpContext())
            {
                scene = context.ImportFile(Path, PostProcessSteps.Triangulate | 
//                                                 PostProcessSteps.FlipUVs | 
                                                 PostProcessSteps.OptimizeMeshes |
                                                 PostProcessSteps.JoinIdenticalVertices);
                processNode(scene, scene.RootNode);
            }
        }

        public vec3 Position { get; set; } = new vec3(0.0f, 0.0f, 0.0f);
        public vec3 Rotation { get; set; } = new vec3(0.0f, 1.0f, 0.0f);
        public vec3 Scale { get; set; } = new vec3(1.0f, 1.0f, 1.0f );
        public mat4 GetModelMatrix()
        {
            var m = mat4.identity();
            m = glm.scale(m, Scale);
            m = glm.translate(m, Position);

            return Parent?.GetModelMatrix() ?? m;
        }

        public void Update(float deltaTime)
        {
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            return childs;
        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            yield return new TextureMaterial().Program;
        }

        private void processNode(Assimp.Scene scene, Node node)
        {
            if (node.HasMeshes)
            {
                foreach (var meshIndex in node.MeshIndices)
                {
                    var mesh = scene.Meshes[meshIndex];
                    processMesh(mesh);
                }
            }
            
            if (node.HasChildren)
            {
                foreach (var child in node.Children)
                {
                    processNode(scene, child);
                }
            }
        }

        private void processMesh(Assimp.Mesh mesh)
        {
            Mesh m;

            var mt = scene.Materials[mesh.MaterialIndex];

            if (false)
            {
                var geometry = new IndexedColorGeometry();
                var list = new List<ColorVertex>();

                for (var i = 0; i < mesh.VertexCount; i++)
                {
                    var v = mesh.Vertices[i];

                    var color = new Color4D(1.0f, 1.0f, 1.0f, 1.0f);

                    color = mt.ColorDiffuse;

                    list.Add(new ColorVertex(new vec3(v.X, v.Y, v.Z), new vec4(color.R, color.G, color.B, color.A)));
                }

                geometry.UpdateData(list.ToRawArray());
                
                var indices = new List<int>();
                for(var i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    // retrieve all indices of the face and store them in the indices vector
                    for (var j = 0; j < face.IndexCount; j++)
                    {
                        indices.Add(face.Indices[j]);
                    }
                }
                geometry.UpdateIndicies(indices.ToArray());
                
                m = new Mesh(geometry, new ColorMaterial());
            }
            else
            {
                var geometry = new IndexedTextureGeometry();
                var list = new List<TextureVertex>();

                for (var i = 0; i < mesh.VertexCount; i++)
                {
                    var v = mesh.Vertices[i];
                    var n = mesh.Normals[i];
                    var t = mesh.TextureCoordinateChannels[0][i];

                    list.Add(new TextureVertex(v.X, v.Y, v.Z, 0, 0, 0, t.X, t.Y));
                }
                
                geometry.UpdateData(list.ToRawArray());
                var indices = new List<int>();
                for(var i = 0; i < mesh.FaceCount; i++)
                {
                    var face = mesh.Faces[i];
                    // retrieve all indices of the face and store them in the indices vector
                    for (var j = 0; j < face.IndexCount; j++)
                    {
                        indices.Add(face.Indices[j]);
                    }
                }

                Material material;

                if (mt.HasTextureDiffuse)
                {
                    var textureMaterial = new TextureMaterial();
                    textureMaterial.DiffuseTexture = loadTexture(mt.TextureDiffuse.FilePath);
                    
                    if (mt.HasTextureSpecular)
                    {
                        textureMaterial.SpecularTexture = loadTexture(mt.TextureSpecular.FilePath);
                    }

                    material = textureMaterial;
                }
                else
                {
                    var color = new vec3(mt.ColorDiffuse.R, mt.ColorDiffuse.G, mt.ColorDiffuse.B);
                    material = new ColorModelMaterial(color);
                }
                

                geometry.UpdateIndicies(indices.ToArray());
                geometry.Mode = PrimitiveType.Triangles;
                m = new Mesh(geometry, material);
            }

            m.Parent = this;
            childs.Add(m);
        }

        public Scene Clone()
        {
            var l = new List<Mesh>();
            foreach (var child in childs)
            {
                var m = child.Clone();
                l.Add(m);
                m.Parent = this;
            }
            
            return new Scene(l, scene);
        }
    }
}