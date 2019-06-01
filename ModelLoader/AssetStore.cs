using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GameModel;
using GameModel.GameObjects;

namespace ModelLoader
{
    public class AssetStore
    {
        private readonly string _path;
        private readonly Logger _logger;
        public List<Asset> Assets { get; private set; } = new List<Asset>();
        public Action OnAssetUpdate; 
        public AssetStore(string path, Logger logger)
        {
            _logger = logger;
            _path = path;
            LoadAssets();
            AddAsset(new MapAsset
            {
                GameObjectType = typeof(Map).AssemblyQualifiedName,
                Name = "Desert Map",
                MapTexture = "textures/desert.jpeg",
                SkyboxAsset = new SkyboxAsset
                {
                    Front = "textures/skybox/front.jpg",
                    Back = "textures/skybox/back.jpg",
                    Top = "textures/skybox/top.jpg",
                    Bottom = "textures/skybox/bottom.jpg",
                    Right = "textures/skybox/right.jpg",
                    Left = "textures/skybox/left.jpg"
                }
            });
        }

        private void LoadAssets()
        {
            try
            {
                _logger.Info($"Loading assets from file: {_path}");

                using (var reader = new StreamReader(_path))
                {
                    var serializer = new XmlSerializer(Assets.GetType());

                    if (serializer.Deserialize(reader) is List<Asset> assets && assets.Count > 0)
                    {
                        Assets = assets;
                    }
                    else
                    {
                        Assets = new List<Asset>();
                        _logger.Warning($"No assets loaded from file: {_path}");
                    }
                }
            }
            catch (Exception e)
            {
                Assets = new List<Asset>();
                _logger.Error(e);
            }
        }

        private void SaveAssets()
        {
            _logger.Info($"Saving assets to file: {_path}");
            using (var writer = new StreamWriter(_path))
            {
                var serializer = new XmlSerializer(Assets.GetType());
                serializer.Serialize(writer, Assets);
            }
        }

        public void AddAsset(Asset asset)
        {
            try
            {
                if (Assets.Exists(a => a.Name == asset.Name))
                {
                    Assets.Remove(Assets.Find(a => a.Name == asset.Name));
                }
                
                Assets.Add(asset);
                SaveAssets();
                OnAssetUpdate?.Invoke();
            }
            catch(Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}