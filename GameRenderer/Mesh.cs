using System;
using System.Collections.Generic;
using GlmNet;

namespace GameRenderer
{
    public class Mesh : IDrawable
    {
        public bool Visible { get; set; } = true;
        public Geometry Geometry { get; set; }
        public IDrawable Parent { get; set; }
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
        public void Update(float deltaTime)
        {
            // Do nothing
        }

        public Mesh(Geometry geometry, Material material)
        {
            Geometry = geometry;
            Material = material;
        }

        public virtual void Draw()
        {
            if (Visible)
            {
                var matrix = Parent?.GetModelMatrix() ?? GetModelMatrix();
                Material.Program.UniformMat4("model", matrix);
                Material?.Use();
                Geometry?.Draw();
            }
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            yield return this;
        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            yield return Material.Program;
        }

        public Mesh Clone()
        {
            return MemberwiseClone() as Mesh;
        }
    }
}