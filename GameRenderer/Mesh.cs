using System;
using System.Collections.Generic;
using GlmNet;

namespace GameRenderer
{
    public class Mesh
    {
        public Geometry Geometry { get; set; }
        
        public Material Material { get; set; }

        public virtual vec3 Position { get; set; } = new vec3(0.0f, 0.0f, 0.0f);
        
        public virtual vec3 Rotation { get; set; } = new vec3(0.0f, 0.0f, -1.0f);
        public virtual vec3 Scale { get; set; } = new vec3(1.0f, 1.0f, 1.0f);

        protected List<Mesh> childs;

        public Mesh(Geometry geometry, Material material)
        {
            Geometry = geometry;
            Material = material;
            Scale = new vec3(1.0f, 1.0f, 1.0f);
        }

        public mat4 GetModelMatrix()
        {
            var m = mat4.identity();
            
//            m = glm.lookAt(Position, Position + glm.normalize(Rotation), new vec3(0.0f, 1.0f, 0.0f));
            m = glm.scale(m, Scale);
            m = glm.translate(m, Position);

            return m;
        }

        public virtual void Draw()
        {
            Material.Program.UniformMat4("model", GetModelMatrix());
            Material?.Use();
            Geometry?.Draw();
        }

        public void GetMeshes(ref List<Mesh> l)
        {
//            if (l == null)
//            {
//                l = new List<Mesh>();
//            }
//            
//            if (childs != null)
//            {
//                foreach (var child in childs)
//                {
//                    child.GetMeshes(ref l);
//                }
//            }
//            
//            l.Add(this);
        }

        public void GetMaterials(ref List<Material> l)
        {
            if (l == null)
            {
                l = new List<Material>();
            }
            
            if (childs != null)
            {
                foreach (var child in childs)
                {
                    child.GetMaterials(ref l);
                }
            }
            
            l.Add(Material);
        }
        
        public void GetShaders(ref List<ShaderProgram> l)
        {
            if (l == null)
            {
                l = new List<ShaderProgram>();
            }
            
            if (childs != null)
            {
                foreach (var child in childs)
                {
                    child.GetShaders(ref l);
                }
            }
            
            l.Add(Material.Program);
        }
    }
}