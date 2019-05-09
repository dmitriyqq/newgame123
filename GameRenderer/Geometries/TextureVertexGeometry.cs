using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class TextureVertexGeometry : Geometry
    {
        
        public override void DefineData()
        {
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, TextureVertex.ByteSize, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, TextureVertex.ByteSize, 3  * sizeof(float));
            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, TextureVertex.ByteSize, 6  * sizeof(float));
        }

        public override int VertexSize() => TextureVertex.FloatSize;
    }
}