using System;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using GameModel;
using GlmNet;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GameRenderer
{
    public class Scene : IDrawable
    {
        public IDrawable Parent { get; set; }

        private readonly Assimp.Scene _scene;

        private readonly List<Mesh> _children;

        private readonly Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        
        private Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        private Animation currentAnimation = null;
        private bool repeatAnimation = false;
        
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

        public IEnumerable<string> GetAvailableAnimations()
        {
            return Animations.Keys;
        }

        public void StartAnimation(string animation)
        {
            Animations.TryGetValue(animation, out currentAnimation);

            currentAnimation?.Start();
        }

        public void RepeatAnimation(string animation)
        {
            repeatAnimation = true;
            currentAnimation.Start();
        }
        
        public string Name { get; private set; }

        public string Directory { get; private set; }

        public string Path => $"{Directory}/{Name}";
        

        private Texture LoadTexture(string filepath)
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
        
        // Create copy of the scene
        private Scene(List<Mesh> children, Assimp.Scene scene)
        {
            _children = children;
            _scene = scene;

            foreach (var child in children)
            {
                child.Parent = this;
            }
        }
        public Scene(string path)
        {
            _children = new List<Mesh>();
            var segments = new List<string>(path.Split('/'));
            Name = segments[segments.Count - 1]; 
            segments.RemoveAt(segments.Count - 1);
            Directory = string.Join("/", segments);
            
            using (var context = new AssimpContext())
            {
                _scene = context.ImportFile(Path,PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
                ProcessNode(_scene, _scene.RootNode);

                if (_scene.HasAnimations)
                {
                }
            }
        }
        public void Update(float deltaTime)
        {
            currentAnimation?.Update(deltaTime);
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            return _children;
        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            yield return new TextureMaterial().Program;
        }

        private void ProcessNode(Assimp.Scene scene, Node node)
        {
            if (node.HasMeshes)
            {
                foreach (var meshIndex in node.MeshIndices)
                {
                    var mesh = scene.Meshes[meshIndex];
                    ProcessMesh(mesh);
                }
            }
            
            if (node.HasChildren)
            {
                foreach (var child in node.Children)
                {
                    ProcessNode(scene, child);
                }
            }
        }

        private void ProcessMesh(Assimp.Mesh mesh)
        {
            Mesh m;

            var mt = _scene.Materials[mesh.MaterialIndex];

           

            var geometry = new IndexedTextureGeometry();
            var list = new List<TextureVertex>();

            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                var n = mesh.Normals[i];

                vec2 t = new vec2(0);
                if (mesh.TextureCoordinateChannels[0] != null)
                {
                    t.x = mesh.TextureCoordinateChannels[0][i].X;
                    t.y = mesh.TextureCoordinateChannels[0][i].Y;
                }

                list.Add(new TextureVertex(v.X, v.Y, v.Z, n.X, n.Z, n.Y, t.x, t.y));
            }
            
            geometry.UpdateData(list.ToArray().ToRawArray());
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
                var textureMaterial = new LightMaterial();
                textureMaterial.shininess = 32;
                if  (mt.HasTextureDiffuse) {
                    textureMaterial.diffuse = LoadTexture(mt.TextureDiffuse.FilePath);
                    textureMaterial.specular = LoadTexture(mt.TextureDiffuse.FilePath);
                }
                if (mt.HasTextureSpecular)
                {
                    textureMaterial.specular = LoadTexture(mt.TextureSpecular.FilePath);
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
            m = new Mesh(geometry, material) {Name = mesh.Name, Parent = this};

            _children.Add(m);
        }

        public Scene Clone()
        {
            var l = new List<Mesh>();
            foreach (var child in _children)
            {
                var m = child.Clone();
                l.Add(m);
                m.Parent = this;
            }
            
            return new Scene(l, _scene);
        }
    }
}