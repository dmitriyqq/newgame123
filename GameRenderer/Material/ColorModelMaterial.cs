using GlmNet;

namespace GameRenderer
{
    public class ColorModelMaterial : Material
    {
        private static readonly ShaderProgram program;

        private vec3 color;
        
        static ColorModelMaterial()
        {
            program = new ShaderProgram("shaders/color.vert", "shaders/color.frag");
        }
    
        public ColorModelMaterial(vec3 color)
        {
            Program = program;
            this.color = color;
        }
    
        public override void Use()
        {
            program.UniformVec3("color", color);
            program.Use();
        }
    }
}