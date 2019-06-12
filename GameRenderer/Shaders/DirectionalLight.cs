using GameRenderer.Shaders;
using GlmNet;

namespace GameRenderer
{
    public class DirectionalLight : Light
    {
        private vec3 direction;
        private vec3 ambient;
        private vec3 diffuse;
        private vec3 specular;

        public vec3 Direction
        {
            get => direction;
            set => direction = value;
        }
        public vec3 Ambient
        {
            get => ambient;
            set => ambient = value;
        }
        public vec3 Diffuse
        {
            get => diffuse;
            set => diffuse = value;
        }
        public vec3 Specular
        {
            get => specular;
            set => specular = value;
        }

        public override void Uniform(ShaderProgram program)
        {
            program.UniformVec3("dirLight.ambient", ambient.to_array());
            program.UniformVec3("dirLight.diffuse", diffuse.to_array());
            program.UniformVec3("dirLight.specular", specular.to_array());
            program.UniformVec3("dirLight.direction", direction.to_array());
        }
    }
}