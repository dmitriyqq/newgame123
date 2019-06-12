using System.Collections.Generic;
using GameModel;
using GameRenderer.Metadata.Assets;

namespace GameRenderer
{
    public class AssetsManager
    {
        private readonly Dictionary<string, Asset> _assets = new Dictionary<string, Asset>();
        private readonly Logger _logger;
        
        public AssetsManager(IEnumerable<Asset> assets, Logger logger)
        {
            _logger = logger;
            
            foreach (var asset in assets)
            {
                _assets[asset.Name] = asset;
            }
        }

        public Asset GetAsset(string name)
        {
            if (_assets.TryGetValue(name, out var asset))
            {
                return asset;
            }

            _logger.Warning($"Couldn't find asset with name {name}");
            return null;
        }

        public void AddAsset(string name, Asset asset)
        {
            if (_assets.ContainsKey(name))
            {
                _logger.Warning($"Replacing existing asset with name {name}");    
            }

            _assets[name] = asset;
        }
    }
}