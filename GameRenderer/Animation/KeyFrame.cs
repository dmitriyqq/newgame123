using System.Collections.Generic;

namespace GameRenderer.Animation
{
    public class KeyFrame
    {
        public float TimeStamp { get; }
        public Dictionary<string, JointTransform> Pose { get; }

        public KeyFrame(float timeStamp, Dictionary<string, JointTransform> jointKeyFrames) {
            TimeStamp = timeStamp;
            Pose = jointKeyFrames;
        }
    }
}