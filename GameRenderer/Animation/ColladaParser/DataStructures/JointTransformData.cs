using System;
using GlmNet;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class JointTransformData
    {
        
        public readonly string JointNameId;
        public readonly mat4 JointLocalTransform;

        public JointTransformData(string jointNameId, mat4 jointLocalTransform)
        {
            JointNameId = jointNameId;
            JointLocalTransform = jointLocalTransform;
        }
    }
}