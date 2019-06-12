using GameRenderer.Shaders;
using GlmNet;

namespace GameRenderer
{
    public class PointLight : Light
    {
        private float _constant;
        private float _linear;
        private float _quadratic;
        private vec3 _position;
        private vec3 _ambient;
        private vec3 _diffuse;
        private vec3 _specular;
        public float Constant
        {
            get => _constant;
            set => _constant = value;
        }
        public float Linear
        {
            get => _linear;
            set => _linear = value;
        }
        public float Quadratic
        {
            get => _quadratic;
            set => _quadratic = value;
        }
        public vec3 Position
        {
            get => _position;
            set => _position = value;
        }
        public vec3 Ambient
        {
            get => _ambient;
            set => _ambient = value;
        }
        public vec3 Diffuse
        {
            get => _diffuse;
            set => _diffuse = value;
        }
        public vec3 Specular
        {
            get => _specular;
            set => _specular = value;
        }

        public override void Uniform(ShaderProgram program)
        {
            program.UniformVec3("pointLights.ambient", _ambient);
            program.UniformVec3("pointLights.diffuse", _diffuse);
            program.UniformVec3("pointLights.specular", _specular);
            program.UniformVec3("pointLights.position", _position);
            program.UniformFloat("pointLights.constant", _constant);
            program.UniformFloat("pointLights.linear", _linear);
            program.UniformFloat("pointLights.quadratic", _quadratic);
        }
    }
}