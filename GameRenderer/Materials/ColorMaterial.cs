namespace GameRenderer.Materials
{
    public class ColorMaterial : ShaderMaterial
    {
        private static ShaderProgram program ;

        static ColorMaterial()
        {
            program = new ShaderProgram("shaders/debug.vert", "shaders/debug.frag");
        }

        public ColorMaterial() : base(program)
        {
        }

        public override void Use()
        {
            Program.Use();
        }
    }
}