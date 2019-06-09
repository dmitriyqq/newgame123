using System;
using System.Xml.Serialization;

namespace ModelLoader
{
    [Serializable]
    [XmlInclude(typeof(SimpleAsset))]
    [XmlInclude(typeof(SkyboxAsset))]
    [XmlInclude(typeof(MapAsset))]
    [XmlInclude(typeof(AnimatedModelAsset))]
    [XmlInclude(typeof(BoxAsset))]
    public class Asset
    {
        [XmlElement]
        public string GameObjectType { get; set; }
        public string Name { get; set; }
    }
}