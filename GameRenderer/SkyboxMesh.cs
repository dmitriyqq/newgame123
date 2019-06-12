using GameRenderer.Materials;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class SkyboxMesh : Mesh
    {
        public SkyboxMesh(Geometry geometry, Material material)
        {
            Geometry = geometry;
            Material = material;
            UseDepth = false;
        }

        public override void Draw()
        {
            GL.Disable(EnableCap.DepthTest);
            base.Draw();
            GL.Enable(EnableCap.DepthTest);
        }
    }
}