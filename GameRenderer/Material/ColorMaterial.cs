namespace GameRenderer
{
    public class ColorMaterial : Material
    {
        private static ShaderProgram program ;

        static ColorMaterial()
        {
            program = new ShaderProgram("shaders/debug.vert", "shaders/debug.frag");
        }

        public ColorMaterial()
        {
            Program = program;
        }

        public override void Use()
        {
            program.Use();
        }
    }
}