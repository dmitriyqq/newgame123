namespace GameRenderer.Materials
{
    public class ParticleMaterial : ShaderMaterial
    {
        private static readonly ParticleShader program;
        
        static ParticleMaterial()
        {
            program = new ParticleShader("shaders/particles.vert", "shaders/particles.frag");
        }

        public ParticleMaterial() : base(program)
        {
        }

        public override void Use()
        {
            program.Use();
        }
    }
}