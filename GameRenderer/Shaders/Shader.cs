using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class Shader
    {
        public int Id { get; }

        public Shader(string filename, ShaderType shaderType)
        {
            var sr = new StreamReader(filename);
            var source = sr.ReadToEnd();
            
            Id = GL.CreateShader(shaderType);
            
            GL.ShaderSource(Id, 1, new [] {source}, new []{source.Length});
            GL.CompileShader(Id);

            //Error checking
            var infolog = GL.GetShaderInfoLog(Id);
            Console.WriteLine($"Infolog for shader name={filename} id={Id}: {infolog}");
        }
    }
}