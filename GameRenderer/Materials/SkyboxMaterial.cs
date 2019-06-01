using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.Materials
{
    public class SkyboxMaterial : ShaderMaterial
    {
        /*
            Correct order:
            right,
            left
            top,
            bottom,
            front,
            back
         */
        
        private static readonly ShaderProgram program;
        public SkyboxTexture  Texture { get; set; }
        
        static SkyboxMaterial()
        {
            program = new ShaderProgram("shaders/cubemap.vert", "shaders/cubemap.frag");
        }

        public SkyboxMaterial() : base(program)
        {
        }
        public override void Use()
        {
            program.Use();
            Texture?.Use(TextureUnit.Texture0);
        }
    }
}