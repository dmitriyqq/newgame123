using System.Collections.Generic;
using GameModel;
using GameModel.GameObjects;
using GameRenderer.Materials;
using GameUtils;
using GlmNet;
using ModelLoader;

namespace GameRenderer
{
    public class DrawablesFactory
    {
        private readonly AssetStore _store;
        private readonly Logger _logger;
        private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
        private readonly Dictionary<string, Material> _loadedMaterials = new Dictionary<string, Material>();
        private readonly Renderer _renderer;
        
        public DrawablesFactory(AssetStore store, Renderer renderer, Logger logger)
        {
            _logger = logger;
            _store = store;
            _renderer = renderer;
        }

        public IDrawable CreateDrawableForGameObject(GameObject gameObject)
        {
            if (gameObject.Asset is Asset asset)
            {
                return CreateDrawableForAsset(asset, gameObject);
            }

            asset = FindAssetForObject(gameObject.GetType().AssemblyQualifiedName);
            if (asset != null) return CreateDrawableForAsset(asset, gameObject);

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
                    
            }

            return null;
        }

        private Material GetOrCreateMaterial(string materialType)
        {
            if (_loadedMaterials.ContainsKey(materialType))
            {
                return ReflectionHelper.CreateObjectFromType<Material>(materialType);
              
            }

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
            foreach (var asset in _store.Assets)
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