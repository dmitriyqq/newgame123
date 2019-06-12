using System.Collections.Generic;
using GlmNet;

namespace GameRenderer
{
    public interface IDrawable
    {
        IDrawable Parent { get; set; }
        vec3 Position { get; set; }
        vec3 Rotation { get; set; }
        vec3 Scale { get; set; }
        bool Visible { get; set; }
        mat4 GetModelMatrix();
        IEnumerable<Mesh> GetAllMeshes();
        void Update(float deltaTime);
    }
}