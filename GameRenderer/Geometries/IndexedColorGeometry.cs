using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class IndexedColorGeometry : ColorVertexGeometry
    {
        private int Ebo = -1;

        private int count;

        public void UpdateIndicies(int[] indicies)
        {
            UseVao();
            Ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Length * sizeof(int), indicies, BufferUsageHint.StaticDraw);
            count = indicies.Length;
        }

        public override void Draw()
        {
            UseVao();
            GL.DrawElements(PrimitiveType.Quads, count, DrawElementsType.UnsignedInt, 0);
        }

        ~IndexedColorGeometry()
        {
            if (Ebo != -1)
            {
//                GL.DeleteBuffer(Ebo);
            }
        }
    }
}