namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class MeshData
    {
        public readonly float[] Vertices;
        public readonly float[] TextureCoords;
        public readonly float[] Normals;
        public readonly int[] Indices;
        public readonly int[] JointIds;
        public readonly float[] VertexWeights;

        public MeshData(float[] vertices, float[] textureCoords, float[] normals, int[] indices,
            int[] jointIds, float[] vertexWeights)
        {
            Vertices = vertices;
            TextureCoords = textureCoords;
            Normals = normals;
            Indices = indices;
            JointIds = jointIds;
            VertexWeights = vertexWeights;
        }
    }
}