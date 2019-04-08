using GlmNet;

namespace GameRenderer
{
    public class TextureMaterial : Material
    {
        private static readonly ShaderProgram program;
        
        public vec3 Color { get; set; }

        static TextureMaterial()
        {
            program = new ShaderProgram("shaders/texture.vert", "shaders/texture.frag");
        }

        public TextureMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
//            program.UniformVec4("color", Color);
            program.Use();
        }
    }
}