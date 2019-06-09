namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class AnimationData
    {
        public readonly float LengthSeconds;
        public readonly KeyFrameData[] KeyFrames;

        public AnimationData(float lengthSeconds, KeyFrameData[] keyFrames) 
        {
            LengthSeconds = lengthSeconds;
            KeyFrames = keyFrames;
        }
    }
}