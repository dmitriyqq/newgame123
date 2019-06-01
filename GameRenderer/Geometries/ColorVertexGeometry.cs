using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class ColorVertexGeometry : OpenGLGeometry
    {
        public override void DefineData()
        {
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, ColorVertex.ByteSize, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, ColorVertex.ByteSize, 3  * sizeof(float));
        }

        public override int VertexSize()
        {
            return ColorVertex.FloatSize;
        }
    }
}