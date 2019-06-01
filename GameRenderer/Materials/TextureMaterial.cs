using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.Materials
{
    public class TextureMaterial : ShaderMaterial
    {
        private static readonly ShaderProgram program;
        
        static TextureMaterial()
        {
            program = new ShaderProgram("shaders/texture.vert", "shaders/texture.frag");
        }

        public TextureMaterial() : base(program)
        {
        }

        public override void Use()
        {
            program.Use();
            DiffuseTexture?.Use(TextureUnit.Texture0);
            SpecularTexture?.Use(TextureUnit.Texture1);
        }

        public Texture DiffuseTexture { get; set; }
        
        public Texture SpecularTexture { get; set; }
    }
}