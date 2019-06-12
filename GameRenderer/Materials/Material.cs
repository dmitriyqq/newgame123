using System.Collections;
using GameRenderer.OpenGL;
using GlmNet;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace GameRenderer.Materials
{
    public class Material
    {
        private readonly ArrayList _uniforms;
        private readonly MaterialDescription _materialDescription;
        private readonly int _modelUniformLocation = -1;
        public readonly Texture[] Textures;
        
        public Material(MaterialDescription description)
        {
            _materialDescription = description;
            _uniforms = ArrayList.Repeat(null, description.Asset.Uniforms.Length);

            if (_materialDescription.UniformLocations.ContainsKey(Constants.ModelUniform))
            {
                _modelUniformLocation = _materialDescription.UniformLayout[Constants.ModelUniform];
            }

            if (description.MaxTextures != 0)
            {
                Textures = new Texture[description.MaxTextures];
            }
        }

        public void Uniform(string name, object o)
        {
            if (_materialDescription.UniformLayout.TryGetValue(name, out var layout))
            {
                _uniforms[layout] = o;
            }
        }

        public void UniformModel(mat4 model)
        {
            _uniforms[_modelUniformLocation] = model.to_array();
        }

        public void Use()
        {
            _materialDescription.Program.Use();
            _materialDescription.Uniform(_uniforms);

            if (Textures == null)
            {
                return;
            }

            for (var i = 0; i < _materialDescription.MaxTextures; i++)
            {
                var texture = Textures[i];

                if (texture == null)
                {
                    break;
                }

                texture.Use(TextureUnit.Texture0 + i);
            }
        }
    }
}