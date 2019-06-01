using System;
using System.Collections.Generic;
using Assimp;
using GameRenderer.Materials;
using GlmNet;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GameRenderer
{
    public class Scene : IDrawable
    {
        private readonly Assimp.Scene _scene;
        private readonly List<Mesh> _children;
        private readonly Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        private readonly Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        private readonly Material _material;
        private Animation _currentAnimation;
        private bool _repeatAnimation;
        public IDrawable Parent { get; set; }
        public vec3 Position { get; set; } = new vec3(0.0f, 0.0f, 0.0f);
        public vec3 Rotation { get; set; } = new vec3(0.0f, 1.0f, 0.0f);
        public vec3 Scale { get; set; } = new vec3(1.0f, 1.0f, 1.0f );
        public string Name { get; }
        public string Directory { get; }
        public string Path => $"{Directory}/{Name}";
        public bool Visible
        {
            get
            {
                foreach (var child in _children)
                {
                    if (child.Visible) return true;
                }

                return false;
            }
            set
            {
                foreach (var child in _children)
                {
                    child.Visible = value;
                }
            }
        }

        public mat4 GetModelMatrix()
        {
            var m = mat4.identity();
            m = glm.scale(m, Scale);
            m = glm.translate(m, Position);
            m = glm.rotate(m, (float)- Math.PI / 2.0f, Rotation);

            return Parent?.GetModelMatrix() ?? m;
        }

        public IEnumerable<string> GetAvailableAnimations()
        {
            return _animations.Keys;
        }

        public void StartAnimation(string animation)
        {
            _animations.TryGetValue(animation, out _currentAnimation);

            _currentAnimation?.Start();
        }

        public void RepeatAnimation(string animation)
        {
            _repeatAnimation = true;
            _currentAnimation.Start();
        }

        private Texture LoadTexture(string filepath)
        {
            var f = $"{Directory}/{filepath}";
            if (_textures.ContainsKey(f))
            {
                return _textures[f];
            }

            var t = new Texture(f);
            _textures[f] = t;
            return t;
        }
        
        // Create copy of the scene
        private Scene(List<Mesh> children, Assimp.Scene scene, Material material)
        {
            _children = children;
            _scene = scene;
            _material = material;

            foreach (var child in children)
            {
                child.Parent = this;
            }
        }
        public Scene(string path, Material material)
        {
            _material = material;
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
            _currentAnimation?.Update(deltaTime);
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            return _children;
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
            var geometry = new IndexedTextureGeometry();
            var list = new List<TextureVertex>();

            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                var n = mesh.Normals[i];

                var t = new vec2(0);
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

            var material = _material ?? CreateMaterial(mesh);
            
        

            geometry.UpdateIndicies(indices.ToArray());
            geometry.Mode = PrimitiveType.Triangles;

            var m = new Mesh(geometry, material) {Name = mesh.Name, Parent = this};
            _children.Add(m);
        }

        private Material CreateMaterial(Assimp.Mesh mesh)
        {
            var mt = _scene.Materials[mesh.MaterialIndex];
            if (mt.HasTextureDiffuse)
            {
                var textureMaterial = new LightMaterial {Shininess = 32};

                if  (mt.HasTextureDiffuse) {
                    textureMaterial.Diffuse = LoadTexture(mt.TextureDiffuse.FilePath);
                    textureMaterial.Specular = LoadTexture(mt.TextureDiffuse.FilePath);
                }
                if (mt.HasTextureSpecular)
                {
                    textureMaterial.Specular = LoadTexture(mt.TextureSpecular.FilePath);
                }

                return textureMaterial;
            }

            var color = new vec3(mt.ColorDiffuse.R, mt.ColorDiffuse.G, mt.ColorDiffuse.B);
            return new ColorModelMaterial(color);
        }
        
        public Scene Clone(Material material)
        {
            var l = new List<Mesh>();
            foreach (var child in _children)
            {
                var m = child.Clone();
                l.Add(m);
                m.Parent = this;
            }
            
            return new Scene(l, _scene, material ?? _material);
        }
    }
}