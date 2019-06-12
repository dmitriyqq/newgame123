using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using GameRenderer.Metadata.Assets;
using GameRenderer.Shaders;
using GlmNet;

namespace GameRenderer.Materials
{
    public class MaterialDescription
    {
        private readonly Logger _logger;
        private readonly ArrayList _uniforms;

        public readonly ShaderProgram Program;
        public readonly Dictionary<string, int> UniformLocations;
        public readonly Dictionary<string, int> UniformLayout;
        public readonly Dictionary<int, Type> UniformTypes;
        public readonly Dictionary<int, Action<int, object>> UniformFunctions;
        public readonly MaterialAsset Asset;
        public readonly int MaxTextures;
        
        public MaterialDescription(MaterialAsset asset, Logger logger)
        {
            Asset = asset;
            _logger = logger;
            MaxTextures = asset.MaxTextures;
            
            var uniformCount = asset.Uniforms.Length;
            UniformFunctions = new Dictionary<int, Action<int, object>>();
            UniformLocations = new Dictionary<string, int>();
            UniformTypes = new Dictionary<int, Type>();
            UniformLayout = new Dictionary<string, int>();

            _uniforms = ArrayList.Repeat(null, uniformCount);
            
            Program = new ShaderProgram(asset.VertexShaderPath, asset.FragmentShaderPath, logger);
            
            _logger.Info($"Created new {nameof(MaterialDescription)} uniformCount = {uniformCount}");

            var index = 0;
            foreach (var uniform in asset.Uniforms)
            {
                var location = Program.GetUniformLocation(uniform.Name);
                if (location == -1)
                {
                    _logger.Warning($"Shader doesn't support uniform {uniform.Name}");
                    continue;
                }

                UniformLayout[uniform.Name] = index;
                index++;
                
                var type = Type.GetType(uniform.Type);
                _logger.Info($"Supports uniform {uniform.Name}: {type}, location = {location}");
                
                UniformLocations[uniform.Name] = location;
                UniformTypes[location] = type;
                UniformFunctions[location] = GetFunction(type);
            }
        }

        private Action<int, object> GetFunction(Type type)
        {
            switch (type)
            {
                case Type floatType when floatType == typeof(float):
                    return Program.UniformFloat;
                case Type intType when intType == typeof(int):
                    return Program.UniformInt;
                case Type vec3Type when vec3Type == typeof(vec3):
                    return Program.UniformVec3;
                case Type vec4Type when vec4Type == typeof(vec4):
                    return Program.UniformVec4;
                case Type mat4Type when mat4Type == typeof(mat4):
                    return Program.UniformMat4;
                case Type mat4ArrayType when mat4ArrayType == typeof(mat4[]):
                    return Program.UniformMat4Array;
                default:
                    var message = $"Type {type} is not supported";
                    _logger.Error(message);
                    throw new NotImplementedException(message);
            }    
        }
        
        public Material CreateInstance()
        {
            return new Material(this);
        }

        public void Uniform(ArrayList array)
        {
            var index = 0;
            foreach (var uniformFunction in UniformFunctions)
            {
                var newValue = array[index];
                if (newValue == null || newValue == _uniforms[index])
                {
                    index++;
                    continue;
                }

                _uniforms[index] = newValue;
                uniformFunction.Value(uniformFunction.Key, newValue);
                index++;
            }
        }
    }
}