using GlmNet;

namespace GameRenderer.Materials
{
    public class ColorModelMaterial : ShaderMaterial
    {
        private static readonly ShaderProgram program;

        private readonly vec3 _color;
        
        static ColorModelMaterial()
        {
            program = new ShaderProgram("shaders/color.vert", "shaders/color.frag");
        }
    
        public ColorModelMaterial(vec3 color) : base(program)
        {
            _color = color;
        }
        
        public ColorModelMaterial() : base(program)
        {
        }
    
        public override void Use()
        {
            program.UniformVec3("color", _color);
            program.Use();
        }
    }
}