using System;
using System.Collections.Generic;
using System.Data;
using GameModel;
using GameModel.GameObjects;
using GameRenderer.Animation;
using GameRenderer.Animation.ColladaParser.Loader;
using GameRenderer.Materials;
using GameRenderer.Metadata.Assets;
using GameRenderer.OpenGL;
using GameUtils;
using GlmNet;
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
        private readonly MaterialManager _materialManager;
        private readonly AssetsManager _assetsManager;
        private readonly Renderer _renderer;
        
        public DrawablesFactory(Renderer renderer, MaterialManager materialManager, AssetsManager assetsManager, Logger logger)
        {
            _materialManager = materialManager;
            _assetsManager = assetsManager;
            _logger = logger;
            _renderer = renderer;
            _loader = new AnimatedModelLoader(_logger);
        }

        public IDrawable CreateDrawableForGameObject(GameObject gameObject)
        {
            if (gameObject.Asset != null)
            {
                return CreateDrawableForAsset(_assetsManager.GetAsset(gameObject.Asset), gameObject);    
            }

            var asset = FindAssetForObject(gameObject.GetType().AssemblyQualifiedName);

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
                case StaticModelAsset staticModel:
                {
                    var material = _materialManager.GetMaterial(staticModel.MaterialName);
                    var scene = CreateOrCloneScene(staticModel.Scene, material, staticModel.Texture);
                    return scene;
                }
                case MapAsset mapAsset:
                {
                    if (gameObject is Map map)
                    {
                        return new MapDrawable(mapAsset, map, _logger, _materialManager);                        
                    }
                    
                    throw new ArgumentException("Map asset only applies to map object");
                }
                case AnimatedModelAsset animAsset:
                {
                    // Adds material to the renderer to get camera uniforms in the shader
                    if (_animatedScenes.ContainsKey(animAsset.ColladaFile))
                    {
                        return _animatedScenes[animAsset.ColladaFile];
                    }
                    
                    var animatedModel = LoadAnimatedModel(animAsset.ColladaFile, animAsset.Texture, animAsset.MaterialName);
                    _animatedScenes[animAsset.ColladaFile] = animatedModel;
                    animatedModel.DoAnimation(animatedModel.Animations[0]);
                    return animatedModel;
                }
            }

            return null;
        }

        private AnimatedModel LoadAnimatedModel(string animatedModel, string textureFile, string material)
        {
            return _loader.LoadEntity(animatedModel, textureFile, _materialManager.GetMaterial(material));
        }

        private Scene CreateOrCloneScene(string path, Material material, string texture)
        {
            if (_scenes.TryGetValue(path, out var scene))
            {
                return scene.Clone();
            }
            
            scene = new Scene(path, material, texture);
            _scenes[path] = scene;
            return scene.Clone();
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