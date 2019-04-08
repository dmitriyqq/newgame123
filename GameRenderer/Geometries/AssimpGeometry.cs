using System;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class AssimpGeometry
    {
        public int Vao;

        public int Vbo;

        public int Nbo;

        public int Tbo;
        public AssimpGeometry(int numVertices, IntPtr Vertices, IntPtr Normals, IntPtr TexturesCoords)
        {
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            
            Vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.A, Vbo);
        }
    }
}