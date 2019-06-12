using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GameModel;
using GameModel.GameObjects;
using GameRenderer;
using GameRenderer.Metadata;
using GameRenderer.Metadata.Assets;
using GameRenderer.OpenGL;
using GameRenderer.Shaders;
using GlmNet;

namespace ModelLoader
{
    public class AssetStore
    {
        private readonly string _path;
        private readonly Logger _logger;

        public Action OnAssetUpdate;
        public AssetsFile AssetsFile { get; private set; }
        public AssetStore(string path, Logger logger)
        {
            _logger = logger;
            _path = path;
            AssetsFile = new AssetsFile();
            LoadAssets();
            GenerateDefaultAssets();
        }

        private void LoadAssets()
        {
            try
            {
                _logger.Info($"Loading assets from file: {_path}");

                using (var reader = new StreamReader(_path))
                {
                    var serializer = new XmlSerializer(AssetsFile.GetType());

                    if (serializer.Deserialize(reader) is AssetsFile assets)
                    {
                        // List<Asset> assets && assets.Count > 0
                        AssetsFile = assets;
                    }
                    else
                    {
                        _logger.Warning($"No assets loaded from file: {_path}");
                        GenerateDefaultAssets();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                GenerateDefaultAssets();
            }
        }

        private void GenerateDefaultAssets()
        {
            AssetsFile = new AssetsFile();

            AddMaterial(new MaterialAsset
            {
                Name = "AnimatedMaterial",
                VertexShaderPath = "shaders/animated.vert",
                FragmentShaderPath = "shaders/animated.frag",
                MaxTextures = 1,
                ShaderType = typeof(ShaderProgram).AssemblyQualifiedName,
                GeometryLayout = new []
                {
                    GlslInputType.Vec3,
                    GlslInputType.Vec2,
                    GlslInputType.Vec3,
                    GlslInputType.Ivec3,
                    GlslInputType.Vec3
                },
                Uniforms = new []
                {
                    new Uniform{Name = "jointTransforms", Type = typeof(mat4[]).AssemblyQualifiedName},
                    new Uniform{Name = "model", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "view", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "projection", Type = typeof(mat4).AssemblyQualifiedName},
                }
            }, false);         
            
            AddMaterial(new MaterialAsset
            {
                Name = "LightMaterial",
                VertexShaderPath = "shaders/light.vert",
                FragmentShaderPath = "shaders/light.frag",
                MaxTextures = 2,
                ShaderType = typeof(ShaderProgram).AssemblyQualifiedName,
                GeometryLayout = new []
                {
                    GlslInputType.Vec3,
                    GlslInputType.Vec2,
                    GlslInputType.Vec3,
                    GlslInputType.Ivec3,
                    GlslInputType.Vec3
                },
                Uniforms = new []
                {
                    new Uniform{Name = "model", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "view", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "projection", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "material.shininess", Type = typeof(float).AssemblyQualifiedName},
                    new Uniform{Name = "material.specular", Type = typeof(int).AssemblyQualifiedName},
                    new Uniform{Name = "material.diffuse", Type = typeof(int).AssemblyQualifiedName}
                }
            }, false);    
            
            AddMaterial(new MaterialAsset
            {
                Name = "SkyboxMaterial",
                VertexShaderPath = "shaders/cubemap.vert",
                FragmentShaderPath = "shaders/cubemap.frag",
                MaxTextures = 1,
                ShaderType = typeof(ShaderProgram).AssemblyQualifiedName,
                GeometryLayout = new []
                {
                    GlslInputType.Vec3
                },
                Uniforms = new []
                {
                    new Uniform{Name = "model", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "view", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "projection", Type = typeof(mat4).AssemblyQualifiedName},
                }
            }, false);   
            
            AddMaterial(new MaterialAsset
            {
                Name = Constants.DebugMaterial,
                VertexShaderPath = "shaders/debug.vert",
                FragmentShaderPath = "shaders/debug.frag",
                MaxTextures = 0,
                ShaderType = typeof(ShaderProgram).AssemblyQualifiedName,
                GeometryLayout = new []
                {
                    GlslInputType.Vec3,
                    GlslInputType.Vec4
                    
                },
                Uniforms = new []
                {
                    new Uniform{Name = "model", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "view", Type = typeof(mat4).AssemblyQualifiedName},
                    new Uniform{Name = "projection", Type = typeof(mat4).AssemblyQualifiedName},
                }
            }, false);
            
            AddAsset(new MapAsset
            {
                Name = "Desert Map Asset",
                GameObjectType = typeof(Map).AssemblyQualifiedName,
                DiffuseTexture = "textures/desert.jpeg",
                SpecularTexture = "textures/desert.jpeg",
                MaterialName = "LightMaterial",
                SkyboxAsset = new SkyboxAsset
                {
                    Name = "Day Skybox",
                    Right = "textures/skybox/right.jpg",
                    Left = "textures/skybox/left.jpg",
                    Top = "textures/skybox/top.jpg",
                    Bottom = "textures/skybox/bottom.jpg",
                    Front = "textures/skybox/front.jpg",
                    Back = "textures/skybox/back.jpg",
                    GameObjectType = typeof(Map).AssemblyQualifiedName,
                    MaterialName = "SkyboxMaterial"
                },
                MaterialParameters = new List<UniformValue>
                {
                    new UniformValue { Uniform = "material.shininess", Value = 32.0f },
                    new UniformValue { Uniform = "material.specular", Value = 1 },
                    new UniformValue { Uniform = "material.diffuse", Value = 0 }
                }
            }, false);

            SaveAssets();
        }
        
        private void SaveAssets()
        {
            _logger.Info($"Saving assets to file: {_path}");
            using (var writer = new StreamWriter(_path))
            {
                var serializer = new XmlSerializer(AssetsFile.GetType());
                serializer.Serialize(writer, AssetsFile);
            }
        }

        public void AddAsset(Asset asset, bool save = true)
        {
            var list = AssetsFile.Assets;
            try
            {
                if (list.Exists(a => a.Name == asset.Name))
                {
                    list.Remove(list.Find(a => a.Name == asset.Name));
                }
                
                list.Add(asset);
                if (save)
                {
                    SaveAssets();
                }

                OnAssetUpdate?.Invoke();
            }
            catch(Exception e)
            {
                _logger.Error(e);
            }
        }

        private void AddMaterial(MaterialAsset asset, bool save)
        {
            var list = AssetsFile.Materials;
            try
            {
                if (list.Exists(a => a.Name == asset.Name))
                {
                    list.Remove(list.Find(a => a.Name == asset.Name));
                }
                
                list.Add(asset);
                if (save)
                {
                    SaveAssets();
                }
                OnAssetUpdate?.Invoke();
            }
            catch(Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
