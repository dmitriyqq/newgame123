namespace GameRenderer
{
    public class TexturedParticlesMaterial : Material
    {
        private static readonly ParticleShader program;
        
        public Texture Texture { get; set; }
        
        static TexturedParticlesMaterial()
        {
            program = new ParticleShader("shaders/tparticles.vert", "shaders/tparticles.frag");
        }

        public TexturedParticlesMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
            Texture.Use();
            program.Use();
        }
    }
}