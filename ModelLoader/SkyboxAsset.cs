namespace ModelLoader
{
    public class SkyboxAsset : Asset
    {
        public string Front { get; set; }
        public string Back { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Right { get; set; }
        public string Left { get; set; }
        public string Texture { get; set; }
            
//        return new List<string>
//        {
//            "textures/skybox/front.jpg",
//            "textures/skybox/back.jpg",
//            "textures/skybox/top.jpg",
//            "textures/skybox/bottom.jpg",
//            "textures/skybox/right.jpg",
//            "textures/skybox/left.jpg",
//        };
    }
}