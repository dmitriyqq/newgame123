using GlmNet;

namespace GameRenderer
{
    public class ParticleMaterial : Material
    {
        private static readonly ParticleShader program;
        
        static ParticleMaterial()
        {
            program = new ParticleShader("shaders/particles.vert", "shaders/particles.frag");
        }

        public ParticleMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
            program.Use();
        }
    }
}