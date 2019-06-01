using System;
using System.Collections.Generic;
using GlmNet;

namespace GameRenderer
{
    
    /// <summary>
    ///  Mesh is single unit of rendering, it should depend only from one material and vertex array object
    /// </summary>
    public class Mesh : IDrawable
    {
        public string Name { get; set; }
        public IDrawable Parent { get; set; }
        public bool Visible { get; set; } = true;
        public bool UseParentTransform { get; set; } = true;
        public Geometry Geometry { get; set; }
        public Material Material { get; set; }      
        public virtual vec3 Position { get; set; } = new vec3(0.0f, 0.0f, 0.0f);
        public virtual vec3 Rotation { get; set; } = new vec3(0.0f, 1.0f, 0.0f);
        public virtual vec3 Scale { get; set; } = new vec3(1.0f, 1.0f, 1.0f);
        public virtual mat4 GetModelMatrix()
        {
            var m = mat4.identity();
            m = glm.scale(m, Scale);
            m = glm.translate(m, Position);

            return m;
        }


        public virtual void Update(float deltaTime)
        {
            // Do nothing
            // Method should be implemented because Mesh is IDrawable
        }

        public Mesh(Geometry geometry, Material material)
        {
            Geometry = geometry;
            Material = material;
        }

        public virtual void Draw()
        {
            if (!Visible) return;

            var matrix = UseParentTransform && Parent != null ? GetModelMatrix() * Parent.GetModelMatrix(): GetModelMatrix();
            Material.UniformModel(matrix);
            Material?.Use();
            Geometry?.Draw();
        }

        public virtual IEnumerable<Mesh> GetAllMeshes()
        {
            yield return this;
        }

        public virtual Mesh Clone()
        {
            return MemberwiseClone() as Mesh;
        }
    }
}