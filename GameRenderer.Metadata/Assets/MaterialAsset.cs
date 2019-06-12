using ModelLoader;

namespace GameRenderer.Metadata.Assets
{
    public class MaterialAsset
    {
        public string Name { get; set; }
        public string ShaderType { get; set; }
        public string VertexShaderPath { get; set; }
        public string FragmentShaderPath { get; set; }
        public int MaxTextures { get; set; }        
        public GlslInputType[] GeometryLayout { get; set; }
        public Uniform[] Uniforms { get; set; }
    }
}