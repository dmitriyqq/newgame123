using System.Linq;
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
            GL.AttachShader(id, vertShader.Id);
            GL.AttachShader(id, fragShader.Id);
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

        public virtual void UniformMat4Array(string name, mat4[] array)
        {
            Use();
            var layout = GL.GetUniformLocation(id, name);
            var floatsArray = new float[array.Length * 16];
            var index = 0;
            
            foreach (var m in array)
            {
                floatsArray[index + 0] = m[0, 0];
                floatsArray[index + 1] = m[1, 0];
                floatsArray[index + 2] = m[2, 0];
                floatsArray[index + 3] = m[3, 0];
                floatsArray[index + 4] = m[0, 1];
                floatsArray[index + 5] = m[1, 1];
                floatsArray[index + 6] = m[2, 1];
                floatsArray[index + 7] = m[3, 1];
                floatsArray[index + 8] = m[0, 2];
                floatsArray[index + 9] = m[1, 2];
                floatsArray[index + 10] = m[2, 2];
                floatsArray[index + 11] = m[3, 2];
                floatsArray[index + 12] = m[0, 3];
                floatsArray[index + 13] = m[1, 3];
                floatsArray[index + 14] = m[2, 3];
                floatsArray[index + 15] = m[3, 3];
                index += 16;
            }

            GL.UniformMatrix4(layout, array.Length, true, floatsArray);
        }
    }
}