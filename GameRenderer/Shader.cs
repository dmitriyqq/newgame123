using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class Shader
    {
        public int id { get; }

        private int length;

        public Shader(string filename, ShaderType shaderType)
        {
            var sr = new StreamReader(filename);
            var source = sr.ReadToEnd();
            
            id = GL.CreateShader(shaderType);
            
            GL.ShaderSource(id, 1, new [] {source}, new []{source.Length});
            GL.CompileShader(id);

            //Error checking
            var infolog = GL.GetShaderInfoLog(id);
            Console.WriteLine($"Infolog for shader id={id}: {infolog}");
        }
    }
}