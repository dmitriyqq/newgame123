using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class SkyboxMesh : Mesh
    {
        public SkyboxMesh(Geometry geometry, Material material) : base(geometry, material)
        {
        }

        public override void Draw()
        {
            GL.DepthMask(false);
            base.Draw();
            GL.DepthMask(true);
        }
    }
}