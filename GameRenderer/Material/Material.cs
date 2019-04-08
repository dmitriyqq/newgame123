namespace GameRenderer
{
    public abstract class Material
    {
        public ShaderProgram Program { get; set; }
        public abstract void Use();
    }
}