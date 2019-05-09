using System;
using System.Collections.Generic;
using Assimp;
using GameModel;
using GlmNet;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GameRenderer
{
    public class Scene : IDrawable
    {
        public IDrawable Parent { get; set; }

        private Assimp.Scene scene;

        private List<Mesh> childs;

        private Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
        
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
        
        public string Name { get; set; }

        public string Directory { get; set; }

        public string Path => $"{Directory}/{Name}";
        

        private Texture loadTexture(string filepath)
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
        protected Scene(List<Mesh> childs, Assimp.Scene scene)
        {
            this.childs = childs;
            this.scene = scene;

            foreach (var child in childs)
            {
                child.Parent = this;
            }
        }
        public Scene(string directory, string name)
        {
            childs = new List<Mesh>();
            Name = name;
            Directory = directory;
            using (var context = new AssimpContext())
            {
                scene = context.ImportFile(Path,PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
                processNode(scene, scene.RootNode);

                if (scene.HasAnimations)
                {
//                    foreach (var animation in scene.Animations)
//                    {
//                        var a = new Animation();
//                        a.Name = animation.Name;
//                        
//                        if (animation.HasMeshAnimations)
//                        {
////                            foreach (var VARIABLE in animation.)
//                            {
//                                   
//                            }
//                        }
//
//                        if (animation.HasNodeAnimations)
//                        {
//                            
//                        }
//                    }
                }
            }
        }
        public void Update(float deltaTime)
        {
            currentAnimation?.Update(deltaTime);
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
                if  (mt.HasTextureDiffuse) {
                    textureMaterial.DiffuseTexture = loadTexture(mt.TextureDiffuse.FilePath);
                }
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
            m.Name = mesh.Name;

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