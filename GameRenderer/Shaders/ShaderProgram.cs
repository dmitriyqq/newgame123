using GameModel;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.Shaders
{
    public class ShaderProgram
    {
        private readonly int _id;
        private readonly Shader _vertexShader;
        private readonly Shader _fragmentShader;
        private readonly Logger _logger;
        
        public ShaderProgram(string vertexShaderPath, string fragmentShaderPath, Logger logger)
        {
            _logger = logger;
            _vertexShader = new Shader(vertexShaderPath, ShaderType.VertexShader);
            _fragmentShader = new Shader(fragmentShaderPath, ShaderType.FragmentShader);

            _id = GL.CreateProgram();
            GL.AttachShader(_id, _vertexShader.Id);
            GL.AttachShader(_id, _fragmentShader.Id);
            GL.LinkProgram(_id);
            _logger.Info($"Created new {nameof(ShaderProgram)} id = {_id}");
        }

        public void Use()
        {
            GL.UseProgram(_id);
        }

        ~ShaderProgram()
        {
            _logger.Info($"Deleting ${nameof(ShaderProgram)} with id = {_id}");
            GL.DetachShader(_id, _vertexShader.Id);
            GL.DetachShader(_id, _fragmentShader.Id);
            GL.DeleteProgram(_id);
        }

        public int GetUniformLocation(string name) => GL.GetUniformLocation(_id, name);

        // Use mat4.to_array to pass matrix to the shader program
        // to prevent boxing/unboxing of mat4
        public void UniformMat4(string name, object m)
        {
            Use();
            GL.UniformMatrix4(GL.GetUniformLocation(_id, name), 1,false,  (float[]) m);
        }
        
        // Use vec3.to_array to pass matrix to the shader program
        // to prevent boxing/unboxing of vec3
        public void UniformMat4(int location, object m)
        {
            Use();
            GL.UniformMatrix4(location, 1,false,  (float[]) m);
        }

        // Use vec3.to_array to pass matrix to the shader program
        // to prevent boxing/unboxing of vec3
        public void UniformVec3(string name, object v)
        {
            Use();
            GL.Uniform3(GL.GetUniformLocation(_id, name), 1,  (float[]) v);
        }
        
        // Use vec3.to_array to pass matrix to the shader program
        // to prevent boxing/unboxing of vec3
        public void UniformVec3(int location, object v)
        {
            Use();
            GL.Uniform3(location, 1,  (float[]) v);
        }

        public void UniformVec4(string name, object v)
        {
            Use();
            GL.Uniform4(GL.GetUniformLocation(_id, name), 1,  (float[]) v);
        }
        
        public void UniformVec4(int location, object v)
        {
            Use();
            GL.Uniform4(location, 1,  (float[]) v);
        }

        // Todo find a way to solve this without object
        public void UniformFloat(string name, object f)
        {
            Use();
            GL.Uniform1(GL.GetUniformLocation(_id, name), (float) f);
        }

        public void UniformFloat(int location, object v)
        {
            Use();
            GL.Uniform1(location, (float) v);
        }
        
        public void UniformInt(string name, object i)
        {
            Use();
            GL.Uniform1(GL.GetUniformLocation(_id, name), (int) i);
        }
        
        public void UniformInt(int location, object i)
        {
            Use();
            GL.Uniform1(location, (int) i);
        }
        
        public virtual void UniformCamera(Camera camera)
        {
            UniformMat4("view", camera.ViewMatrix.to_array());
            UniformMat4("projection", camera.ProjectionMatrix.to_array());
        }

        public void UniformMat4Array(int location, object ma)
        {
            Use();
            var array = (mat4[]) ma;
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

            GL.UniformMatrix4(location, array.Length, true, floatsArray);
        }
        
        public void UniformMat4Array(string name, object ma)
        {
            var location = GL.GetUniformLocation(_id, name);
            UniformMat4Array(location, ma);
        }
    }
}