using System;
using System.Collections.Generic;
using GameModel;
using GameModel.GameObjects;
using GameRenderer.Animation;
using GameRenderer.Animation.ColladaParser.Loader;
using GameRenderer.Materials;
using GameRenderer.OpenGL;
using GameUtils;
using GlmNet;
using ModelLoader;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class DrawablesFactory
    {
        private readonly Logger _logger;
        private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
        private readonly Dictionary<string, AnimatedModel> _animatedScenes = new Dictionary<string, AnimatedModel>();
        private readonly Dictionary<string, Material> _loadedMaterials = new Dictionary<string, Material>();
        private readonly AnimatedModelLoader _loader;
        private readonly Renderer _renderer;
        
        public DrawablesFactory(Renderer renderer, Logger logger)
        {
            _logger = logger;
            _renderer = renderer;
            _loader = new AnimatedModelLoader(_logger);
        }

        public IDrawable CreateDrawableForGameObject(GameObject gameObject)
        {
            if (gameObject.Asset is Asset asset)
            {
                return CreateDrawableForAsset(asset, gameObject);
            }

            asset = FindAssetForObject(gameObject.GetType().AssemblyQualifiedName);
            if (asset != null)
            {
                return CreateDrawableForAsset(asset, gameObject);
            }

            _logger.Warning($"Couldn't find a drawable for type {gameObject.GetType()}");
            return null;

        }

        public IDrawable CreateDrawableForAsset(Asset asset, GameObject gameObject)
        {
            switch (asset)
            {
                case SimpleAsset simpleAsset:
                {
                    var materialType = simpleAsset.MaterialType;
                    var material = simpleAsset.MaterialType != null ? GetOrCreateMaterial(materialType) : null;
                    var scene = CreateOrCloneScene(simpleAsset.Scene, material);
                    return scene;
                }
                case SkyboxAsset skyboxAsset:
                {
                    var skyboxTexture = new SkyboxTexture(new List<string>
                    {
                        skyboxAsset.Front,
                        skyboxAsset.Back,
                        skyboxAsset.Top,
                        skyboxAsset.Bottom,
                        skyboxAsset.Right,
                        skyboxAsset.Left,
                        skyboxAsset.Texture
                    });
                    var material =  GetOrCreateMaterial(typeof(SkyboxMaterial).AssemblyQualifiedName);
                    (material as SkyboxMaterial).Texture = skyboxTexture;
                    return new SkyboxMesh(new CubeGeometry(), material) {Scale = new vec3(100.0f, 100.0f, 100.0f)};
                }
                case MapAsset mapAsset:
                {
                    var mapTexture = new Texture(mapAsset.MapTexture);
                    var material = GetOrCreateMaterial(typeof(LightMaterial).AssemblyQualifiedName);
                    (material as LightMaterial).Diffuse = mapTexture;
                    (material as LightMaterial).Specular = mapTexture;
                    (material as LightMaterial).Shininess = 32.0f;
                    var mapDrawable = new CompoundMesh(new MapGeometry(gameObject as Map), material);
                    var skyboxDrawable = CreateDrawableForAsset(mapAsset.SkyboxAsset, gameObject) as Mesh;
                    mapDrawable.AddChild(skyboxDrawable);
                    skyboxDrawable.Parent = mapDrawable;
                    return mapDrawable;
                }
                case AnimatedModelAsset animAsset:
                {
                    // Adds material to the renderer to get camera uniforms in the shader
                    var material = GetOrCreateMaterial((new AnimatedMaterial()).GetType().AssemblyQualifiedName);
                    
                    if (_animatedScenes.ContainsKey(animAsset.AnimatedModel))
                    {
                        return _animatedScenes[animAsset.AnimatedModel];
                    }
                    
                    var animatedModel = LoadAnimatedModel(animAsset.AnimatedModel, animAsset.Texture);
                    _animatedScenes[animAsset.AnimatedModel] = animatedModel;
                    animatedModel.DoAnimation(animatedModel.Animations[0]);
                    return animatedModel;
                }
                case BoxAsset boxAsset:
                {
                    var material = GetOrCreateMaterial((new ColorMaterial()).GetType().AssemblyQualifiedName);
                    var geometry = new VertexArray(8, PrimitiveType.Triangles, _logger);
                    geometry.UseIndices(new []
                    {
                        7, 6, 4, 1, 7, 5,
                        2, 7, 3, 1, 3, 7,
                        7, 4, 5, 2, 6, 7,
                        4, 6, 2, 0, 3, 1,
                        3, 0, 2, 0, 1, 5,
                        0, 5, 4, 0, 4, 2
                    });
                    geometry.AttachComponent("positions", BufferUsageHint.StaticDraw, new []
                    {
                        //
                        1.0f, 1.0f, 1.0f, // 0
                        1.0f, 1.0f, -1.0f, // 1
                        1.0f, -1.0f, 1.0f, // 2
                        1.0f, -1.0f, -1.0f, // 3
                        -1.0f, 1.0f, 1.0f, // 4
                        -1.0f, 1.0f, -1.0f, // 5
                        -1.0f, -1.0f, 1.0f, // 6
                        -1.0f, -1.0f, -1.0f, // 7
//                         
//                        // Order
//                        -1.0f,-1.0f,-1.0f, // 7
//                        -1.0f,-1.0f, 1.0f, // 6
//                        -1.0f, 1.0f, 1.0f, // 4 
//                        1.0f, 1.0f,-1.0f, // 1
//                        -1.0f,-1.0f,-1.0f, // 7
//                        -1.0f, 1.0f,-1.0f,  // 5
//                        
//                        1.0f,-1.0f, 1.0f, // 2
//                        -1.0f,-1.0f,-1.0f, // 7
//                        1.0f,-1.0f,-1.0f, // 3
//                        1.0f, 1.0f,-1.0f, // 1
//                        1.0f,-1.0f,-1.0f, // 3
//                        -1.0f,-1.0f,-1.0f, // 7
//                        
//                        -1.0f,-1.0f,-1.0f, // 7
//                        -1.0f, 1.0f, 1.0f, // 4
//                        -1.0f, 1.0f,-1.0f, // 5
//                        1.0f,-1.0f, 1.0f, // 2
//                        -1.0f,-1.0f, 1.0f, // 6
//                        -1.0f,-1.0f,-1.0f, // 7
//                        
//                        -1.0f, 1.0f, 1.0f, // 4
//                        -1.0f,-1.0f, 1.0f, // 6
//                        1.0f,-1.0f, 1.0f, // 2
//                        1.0f, 1.0f, 1.0f, // 0
//                        1.0f,-1.0f,-1.0f, // 3
//                        1.0f, 1.0f,-1.0f, // 1
//                        
//                        1.0f,-1.0f,-1.0f, // 3
//                        1.0f, 1.0f, 1.0f, // 0
//                        1.0f,-1.0f, 1.0f, // 2
//                        1.0f, 1.0f, 1.0f, // 0
//                        1.0f, 1.0f,-1.0f, // 1
//                        -1.0f, 1.0f,-1.0f, // 5
//                        
//                        1.0f, 1.0f, 1.0f, // 0
//                        -1.0f, 1.0f,-1.0f, // 5
//                        -1.0f, 1.0f, 1.0f, // 4
//                        1.0f, 1.0f, 1.0f, // 0
//                        -1.0f, 1.0f, 1.0f, // 4
//                        1.0f,-1.0f, 1.0f // 2
                    }, 3, VertexAttribPointerType.Float, 1);
                    geometry.AttachComponent("Colors", BufferUsageHint.StaticDraw, new []
                    {
                        //
                        1.0f, 0.0f, 0.0f, 1.0f, // 0
                        1.0f, 0.0f, 1.0f, 1.0f,// 1
                        1.0f, 1.0f, 0.0f, 1.0f,// 2
                        1.0f, 0.0f, 1.0f, 1.0f,// 3
                        0.0f, 1.0f, 1.0f, 1.0f,// 4
                        0.0f, 1.0f, 0.0f, 1.0f,// 5
                        1.0f, 1.0f, 0.0f, 1.0f,// 6
                        1.0f, 1.0f, 0.0f, 1.0f,// 7);
                        0.0f, 1.0f, 0.0f, 1.0f
                    }, 4, VertexAttribPointerType.Float, 1);
                    geometry.GenerateVertexAttribPointer();
                    return new Mesh(geometry, material);
                }
            }

            return null;
        }

        private AnimatedModel LoadAnimatedModel(string animatedModel, string textureFile)
        {
            return _loader.LoadEntity(animatedModel, textureFile);
        }

        private Material GetOrCreateMaterial(string materialType)
        {
            if (_loadedMaterials.ContainsKey(materialType))
            {
                _logger.Info($"Material {materialType} already exists");
                return ReflectionHelper.CreateObjectFromType<Material>(materialType);
            }

            _logger.Info($"Creating instance of material {materialType}");
            var material = ReflectionHelper.CreateObjectFromType<Material>(materialType);
            _loadedMaterials[materialType] = material;
            if (material is ShaderMaterial shaderMaterial)
            {
                _renderer.AddMaterial(shaderMaterial);                    
            }

            return ReflectionHelper.CreateObjectFromType<Material>(materialType);
        }

        private Scene CreateOrCloneScene(string path, Material material)
        {
            if (_scenes.TryGetValue(path, out var scene))
            {
                return scene.Clone(material);
            }
            
            scene = new Scene(path, material);
            _scenes[path] = scene;
            return scene.Clone(material);
        }

        private Asset FindAssetForObject(string type)
        {
            foreach (var asset in _renderer.Assets)
            {
                if (type == asset.GameObjectType)
                {
                    return asset;
                }
            }
            
            return null;
        }
    }
}