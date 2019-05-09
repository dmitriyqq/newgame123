using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class LightMaterial: Material
    {
        private static ShaderProgram program ;

        public float shininess;

        public Texture diffuse;

        public Texture specular;
        
        static LightMaterial()
        {
            program = new ShaderProgram("shaders/light.vert", "shaders/light.frag");
        }

        public LightMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
            program.Use();
            program.UniformInt("material.diffuse", 1);
            program.UniformInt("material.specular", 1);
            program.UniformFloat("material.shininess", 32.0f);
            diffuse.Use();
            specular?.Use(TextureUnit.Texture1);
        }
    }
}