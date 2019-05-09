using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class VAO
    {
        private int id = -1;

        public VAO()
        {
            id = GL.GenVertexArray();
        }

        ~VAO()
        {
            if (id != -1)
            {
                GL.DeleteVertexArray(id);
            }
        }
        
        private void Use()
        {
            GL.BindVertexArray(id);
        }

        public void AttachVBO(VBO vbo)
        {
            
        }

        public virtual void Draw()
        {
               
        }
    }
}