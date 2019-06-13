using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameRenderer.Metadata.Assets
{
    [Serializable]
    [XmlInclude(typeof(StaticModelAsset))]
    [XmlInclude(typeof(SkyboxAsset))]
    [XmlInclude(typeof(MapAsset))]
    [XmlInclude(typeof(AnimatedModelAsset))]
    public class Asset
    {
        public string GameObjectType { get; set; }
        public string MaterialName { get; set; }
        public string Name { get; set; }
        public List<UniformValue> MaterialParameters { get; set; }
    }
}