using System;
using System.Collections.Generic;
using System.Numerics;
using GameRenderer.Animation.ColladaParser.DataStructures;
using GameUtils;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class AnimationLoaderWrapper
    {
        private readonly ColladaLoader _colladaLoader;

        public AnimationLoaderWrapper(ColladaLoader loader)
        {
            _colladaLoader = loader;
        }
        
        public Animation LoadAnimation(string colladaFile) {
            var animationData = _colladaLoader.LoadColladaAnimation(colladaFile);
            var frames = new KeyFrame[animationData.KeyFrames.Length];

            for (var i = 0; i < frames.Length; i++) 
            {
                frames[i] = CreateKeyFrame(animationData.KeyFrames[i]);
            }

            return new Animation(animationData.LengthSeconds, frames);
        }

        private KeyFrame CreateKeyFrame(KeyFrameData data) 
        {
            var map = new Dictionary<string, JointTransform>();
            foreach (var jointData in data.JointTransforms) 
            {
                var jointTransform = CreateTransform(jointData);
                map[jointData.JointNameId] = jointTransform;
            }
            return new KeyFrame(data.Time, map);
        }

        private JointTransform CreateTransform(JointTransformData data) 
        {
            var m = data.JointLocalTransform;
            var translation = new Vector3(m[3,0], m[3,1], m[3,2]);
            var rotation = QuaternionHelper.FromMatrix(m);
            return new JointTransform(translation, rotation);
        }

    }
}