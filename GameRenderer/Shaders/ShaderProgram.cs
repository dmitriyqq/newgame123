using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class ShaderProgram
    {
        private int id;

        public ShaderProgram(string vertexShaderPath, string fragmentShaderPath)
        {
            Shader vertShader = new Shader(vertexShaderPath, ShaderType.VertexShader);
            Shader fragShader = new Shader(fragmentShaderPath, ShaderType.FragmentShader);

            id = GL.CreateProgram();
            GL.AttachShader(id, vertShader.id);
            GL.AttachShader(id, fragShader.id);
            GL.LinkProgram(id);
        }

        public void Use()
        {
            GL.UseProgram(id);
        }

        public void UniformMat4(string name, mat4 m)
        {
            Use();
            var location = GL.GetUniformLocation(id, name);
            GL.UniformMatrix4(location, 1,false,  m.to_array());
        }

        public void UniformVec3(string name, vec3 v)
        {
            Use();
            GL.Uniform3(GL.GetUniformLocation(id, name), 1,  v.to_array());
        }

        public void UniformVec4(string name, vec4 v)
        {
            Use();
            GL.Uniform4(GL.GetUniformLocation(id, name), 1,  v.to_array());
        }

        public void UniformFloat(string name, float f)
        {
            Use();
            GL.Uniform1(GL.GetUniformLocation(id, name), f);
        }

        public void UniformInt(string name, int i)
        {
            Use();
            GL.Uniform1(GL.GetUniformLocation(id, name), i);
        }
        
        public virtual void UniformCamera(Camera camera)
        {
            UniformMat4("view", camera.ViewMatrix);
            UniformMat4("projection", camera.ProjectionMatrix);
        }
    }
}