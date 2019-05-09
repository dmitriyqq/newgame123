using System;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class VBO
    {
        protected int id = -1; 
        protected BufferTarget target;

        protected BufferUsageHint hint;
        internal VBO(BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            this.target = target;
            this.hint = hint;

            id = GL.GenBuffer();
            GL.BindBuffer(target, id);
        }
        internal void Bind()
        {
            GL.BindBuffer(target, id);
        }

        ~VBO()
        {
            if (id != -1)
            {
                GL.DeleteBuffer(id);
            }
        }
    }
    public class VBO<T> : VBO where T : struct
    {
        internal VBO(BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw) : base(target, hint)
        {
        }

        private int GetElementSize()
        {
            int size = 0;
            if (typeof(T) is int)
            {
                size = sizeof(int);
            }
            else if (typeof(T) is float)
            {
                size = sizeof(float);
            }
            else
            {
                throw new NotImplementedException();
            }

            return size;
        }

        private void BufferData(T[] data)
        {
            GL.BufferData(target, data.Length * GetElementSize(), data, hint);
        }

        public void UpdateData(int offset, T[] data)
        {
            GL.BindBuffer(target, id);
            GL.BufferSubData(target, (IntPtr) offset, data.Length * GetElementSize(), data);
        }
    }
}