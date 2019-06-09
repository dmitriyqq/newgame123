using GameRenderer.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.Materials
{
    public class LightMaterial: ShaderMaterial
    {
        private static ShaderProgram program ;

        public float Shininess;
        public Texture Diffuse;
        public Texture Specular;
        
        static LightMaterial()
        {
            program = new ShaderProgram("shaders/light.vert", "shaders/light.frag");
        }

        public LightMaterial() : base(program)
        {
        }

        public override void Use()
        {
            program.Use();
            program.UniformInt("material.diffuse", 1);
            program.UniformInt("material.specular", 1);
            program.UniformFloat("material.shininess", Shininess);
            Diffuse.Use();
            Specular?.Use(TextureUnit.Texture1);
        }
    }
}