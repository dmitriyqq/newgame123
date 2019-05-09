using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class CubemapMaterial : Material
    {
        /*
                "right.jpg",
                "left.jpg",
                "top.jpg",
                "bottom.jpg",
                "front.jpg",
                "back.jpg"
         */
        
        private static readonly ShaderProgram program;

        public CubemapTexture Texture { get; set; }
        
        static CubemapMaterial()
        {
            program = new ShaderProgram("shaders/cubemap.vert", "shaders/cubemap.frag");
        }

        public CubemapMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
            program.Use();
            Texture?.Use(TextureUnit.Texture0);
        }
    }
}