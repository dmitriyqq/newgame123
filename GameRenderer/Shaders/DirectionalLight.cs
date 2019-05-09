using GlmNet;

namespace GameRenderer
{
    public class DirectionalLight
    {
        private ShaderProgram program;

        public ShaderProgram Program
        {
            get => program;
            set
            {
                program = value;
            }
        }

        private vec3 direction;

        public vec3 Direction
        {
            get => direction;
            set
            {
                direction = value;
            }
        }

        private vec3 ambient;

        public vec3 Ambient
        {
            get => ambient;
            set
            {
                ambient = value;
            }
        }

        private vec3 diffuse;

        public vec3 Diffuse
        {
            get => diffuse;
            set
            {
                diffuse = value;
            }
        }

        private vec3 specular;

        public vec3 Specular
        {
            get => specular;
            set
            {
                specular = value;
            }
        }

        public void Uniform(ShaderProgram program)
        {
            program.UniformVec3("dirLight.ambient", ambient);
            program.UniformVec3("dirLight.diffuse", diffuse);
            program.UniformVec3("dirLight.specular", specular);
            program.UniformVec3("dirLight.direction", direction);
        }
    }
}