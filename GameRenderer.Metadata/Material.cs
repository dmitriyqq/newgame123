using GlmNet;

namespace GameRenderer
{
    public abstract class Material
    {
        public abstract void Use();

        public abstract void UniformModel(mat4 model);
    }
}