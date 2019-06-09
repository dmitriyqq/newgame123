using System;
using System.Xml;
using GameModel;
using GameRenderer.Animation.ColladaParser.DataStructures;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class ColladaLoader
    {
        private readonly Logger _logger; 
        
        public ColladaLoader(Logger logger)
        {
            _logger = logger;
        }
        
        public AnimatedModelData LoadColladaModel(string colladaFile, int maxWeights) 
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(colladaFile);
                var node = doc.ChildNodes[1];

                var skinLoader = new SkinLoader(node.GetChild("library_controllers"), maxWeights, _logger);
                var skinningData = skinLoader.ExtractSkinData();


                    var jointsLoader = new SkeletonLoader(node.GetChild("library_visual_scenes"), skinningData.jointOrder);
                var jointsData = jointsLoader.ExtractBoneData();

                var g = new GeometryLoader(node.GetChild("library_geometries"), skinningData.verticesSkinData);
                var meshData = g.ExtractModelData();

                return new AnimatedModelData(meshData, jointsData);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }

        public AnimationData LoadColladaAnimation(string colladaFile) 
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(colladaFile);
                var node = doc.ChildNodes[1];
                var animNode = node.GetChild("library_animations");
                var jointsNode = node.GetChild("library_visual_scenes");
                var loader = new AnimationLoader(animNode, jointsNode);
                var animData = loader.ExtractAnimation();
                return animData;
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return null;
        }
    }
}