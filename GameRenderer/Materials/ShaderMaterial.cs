using GlmNet;

namespace GameRenderer.Materials
{
    public abstract class ShaderMaterial : Material
    {
        public readonly ShaderProgram Program;

        public override void UniformModel(mat4 model)
        {
            Program.UniformMat4("model", model);
        }

        protected ShaderMaterial(ShaderProgram program)
        {
            Program = program;
        }
    }
}