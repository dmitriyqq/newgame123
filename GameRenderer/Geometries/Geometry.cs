using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public abstract class Geometry
    {
        protected int vao = -1;

        protected int vbo = -1;

        private bool data = false;
        public PrimitiveType Mode { get; set; } = PrimitiveType.Triangles;
        public int Offset { get; set; }
        public int Count { get; set; }

        public Geometry()
        {
        }
        
        public Geometry(PrimitiveType mode, int offset, int count)
        {
            Mode = mode;
            Offset = offset;
            Count = count;
        }

        protected void UseVao() =>  GL.BindVertexArray(vao);
        
        public virtual void Draw()
        {
            if (Count != 0)
            {
                UseVao();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.DrawArrays(Mode, Offset, Count);
            }
        }

        public void UpdateData(float[] data)
        {
            if (this.data == false)
            {
                BufferData(data);
            }
            else
            {
                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * sizeof(float), data);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            Count = data.Length / VertexSize();
        }
        
        public void BufferData(float[] data)
        {
            GenerateVao();
            DefineData();
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void GenerateVao()
        {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }

        public abstract void DefineData();
        
        public abstract int VertexSize();

        ~Geometry()
        {
            if (vao != -1)
            {
//                GL.DeleteVertexArray(vao);
            }

            if (vbo != -1)
            {
//                GL.DeleteBuffer(vbo);
            }
        }
    }
}