namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class SkeletonData
    {
        	
        public readonly int JointCount;
        public readonly JointData HeadJoint;
	
        public SkeletonData(int jointCount, JointData headJoint)
        {
            JointCount = jointCount;
            HeadJoint = headJoint;
        }
    }
}