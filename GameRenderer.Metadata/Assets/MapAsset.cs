namespace GameRenderer.Metadata.Assets
{
    public class MapAsset : Asset
    {
        public SkyboxAsset SkyboxAsset { get; set; }
        public string DiffuseTexture { get; set; }
        public string SpecularTexture { get; set; }
    }
}