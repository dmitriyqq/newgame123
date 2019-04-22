namespace GameRenderer
{
    public class ParticleShader : ShaderProgram
    {
        public ParticleShader(string vertexShaderPath, string fragmentShaderPath) : base(vertexShaderPath,
            fragmentShaderPath)
        {
        }
        public override void UniformCamera(Camera camera)
        {
            base.UniformCamera(camera);
            UniformVec3("cameraUp", camera.Up);
            UniformVec3("cameraRight", camera.Right);
        }
    }
}