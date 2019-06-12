using System.Collections.Generic;

namespace GameRenderer.Metadata.Assets
{
    // Layout of file with assets
    public class AssetsFile
    {
        public List<TextureAsset> Textures { get; private set; } = new List<TextureAsset>();
        public List<MaterialAsset> Materials { get; private set; } = new List<MaterialAsset>();
        public List<Asset> Assets { get; private set; } = new List<Asset>();
    }
}