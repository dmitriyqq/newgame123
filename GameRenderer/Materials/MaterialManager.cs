using System.Collections.Generic;
using GameModel;
using GameRenderer.Metadata.Assets;

namespace GameRenderer.Materials
{
    public class MaterialManager
    {
        private readonly Dictionary<string, MaterialDescription> _materials = new Dictionary<string, MaterialDescription>();
        private readonly Logger _logger;
        
        public IReadOnlyCollection<MaterialDescription> Materials { get; private set; }

        private void UpdateMaterials()
        {
            Materials = _materials.Values;
        }
        
        public MaterialManager(IEnumerable<MaterialAsset> assets, Logger logger)
        {
            _logger = logger;

            foreach (var asset in assets)
            {
                _materials[asset.Name] = new MaterialDescription(asset, _logger);
            }

            UpdateMaterials();
        }
        
        public Material GetMaterial(string name)
        {
            if (_materials.TryGetValue(name, out var material))
            {
                return material.CreateInstance();
            }

            _logger.Warning($"Couldn't find material for type: {name}");
            return null;
        }

        public void AddMaterial(MaterialAsset asset)
        {
            if (_materials.ContainsKey(asset.Name))
            {
                _logger.Info($"Rewriting material {asset.Name}");
            }
            
            _materials[asset.Name] = new MaterialDescription(asset, _logger);
            UpdateMaterials();
        }
    }
}