using GameRenderer.OpenGL;
using GlmNet;

namespace GameRenderer.Materials
{
    public class AnimatedMaterial : ShaderMaterial
    {
        private static readonly ShaderProgram program;
        private const int MaxJoints = 50;
        private const int DiffuseTexUnit = 0;

        public Texture Diffuse;

        static AnimatedMaterial()
        {
            program = new ShaderProgram("shaders/animated.vert", "shaders/animated.frag");
        }

        public AnimatedMaterial() : base(program)
        {
        }

        public override void Use()
        {
            Diffuse.Use();
            Program.Use();
            Program.UniformVec3("jointTransforms", new vec3());
            var m = mat4.identity();
            m = glm.scale(m, new vec3(0.1f));
            Program.UniformMat4("model", m);
        }
    }
}