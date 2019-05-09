using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class ElementBuffer : VBO<int>
    {
        public ElementBuffer(BufferUsageHint hint = BufferUsageHint.StaticDraw) : base(BufferTarget.ElementArrayBuffer)
        {
        }
    }
}