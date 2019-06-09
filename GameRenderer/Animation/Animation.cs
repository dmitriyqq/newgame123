namespace GameRenderer.Animation
{
    public class Animation
    {
        public float Length { get; }
        public KeyFrame[] KeyFrames { get; }

        public Animation(float lengthInSeconds, KeyFrame[] frames) {
            KeyFrames = frames;
            Length = lengthInSeconds;
        }
    }
}