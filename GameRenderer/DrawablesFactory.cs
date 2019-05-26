using System;
using System.Collections.Generic;
using GameModel;
using ModelLoader;

namespace GameRenderer
{
    public class DrawablesFactory
    {
        private AssetStore _store;
        private Logger _logger;
        
        private readonly Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        public DrawablesFactory(AssetStore store)
        {
            _store = store;
        }

        public IDrawable CreateDrawableForGameObject(GameObject gameObject)
        {
            if (gameObject.Asset is Asset asset)
            {
                return CreateDrawableForAsset(asset);
            }

            asset = FindAssetForObject(gameObject.GetType().FullName);
            if (asset == null)
            {
                _logger.Warning($"Couldn't find drawable for type {gameObject.GetType()}");
                return null;
            }
        
            return CreateDrawableForAsset(asset);
        }

        private IDrawable CreateDrawableForAsset(Asset asset)
        {
            if (asset is SimpleAsset sa)
            {
                var scene = CreateOrCloneScene(sa.Scene);
                return scene;
            }

            return null;
        }

        private Scene CreateOrCloneScene(string path)
        {
            if (scenes.TryGetValue(path, out var scene))
            {
                return scene.Clone();
            }
            
            scene = new Scene(path);
            scenes[path] = scene;
            return scene.Clone();
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