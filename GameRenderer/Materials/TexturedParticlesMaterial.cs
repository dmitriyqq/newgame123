namespace GameRenderer.Materials
{
    public class TexturedParticlesMaterial : ShaderMaterial
    {
        private static readonly ParticleShader program;
        
        public Texture Texture { get; set; }
        
        static TexturedParticlesMaterial()
        {
            program = new ParticleShader("shaders/tparticles.vert", "shaders/tparticles.frag");
        }

        public TexturedParticlesMaterial() : base(program)
        {
        }

        public override void Use()
        {
            Texture.Use();
            program.Use();
        }
    }
}