using GlmNet;

namespace GameRenderer
{
    public class PointLight
    {
        private ShaderProgram program;

        private float constant;
        
        public float Constant
        {
            get => constant;
            set
            {
                constant = value;
            }
        }
        
        private float linear;
        
        public float Linear
        {
            get => linear;
            set
            {
                linear = value;
            }
        }
        
        private float quadratic;

        public float Quadratic
        {
            get => quadratic;
            set
            {
                quadratic = value;
            }
        }

        public ShaderProgram Program
        {
            get => program;
            set
            {
                program = value;
            }
        }

        private vec3 position;

        public vec3 Position
        {
            get => position;
            set
            {
                position = value;
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
            program.UniformVec3("PointLight.ambient", ambient);
            program.UniformVec3("PointLight.diffuse", diffuse);
            program.UniformVec3("PointLight.specular", specular);
            program.UniformVec3("PointLight.position", position);
            program.UniformFloat("PointLight.constant", constant);
            program.UniformFloat("PointLight.linear", linear);
            program.UniformFloat("PointLight.quadratic", quadratic);
        }
    }
}