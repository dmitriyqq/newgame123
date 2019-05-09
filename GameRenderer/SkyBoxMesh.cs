using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class SkyBoxMesh : Mesh
    {
        public SkyBoxMesh(Geometry geometry, Material material) : base(geometry, material)
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