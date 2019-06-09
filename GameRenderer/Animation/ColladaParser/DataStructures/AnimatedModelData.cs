namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class AnimatedModelData
    {
        
        private readonly SkeletonData Joints;
        private readonly MeshData Mesh;
	
        public AnimatedModelData(MeshData mesh, SkeletonData joints){
            Joints = joints;
            Mesh = mesh;
        }
	
        public SkeletonData GetJointsData()
        {
            return Joints;
        }
	
        public MeshData GetMeshData()
        {
            return Mesh;
        }
    }
}