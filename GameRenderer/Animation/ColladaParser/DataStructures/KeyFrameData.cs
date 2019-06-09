using System.Collections.Generic;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class KeyFrameData
    {
        public readonly float Time;
        public readonly List<JointTransformData> JointTransforms = new List<JointTransformData>();
	
        public KeyFrameData(float time)
        {
            Time = time;
        }
	
        public void AddJointTransform(JointTransformData transform)
        {
            JointTransforms.Add(transform);
        }

    }
}