using System.Collections.Generic;
using System.Numerics;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class Vertex
    {
        private const int NoIndex = -1;
        public readonly int Index;
        public readonly float Length;
        public readonly VertexSkinData WeightsData;

        public Vector3 Position;
        public int TextureIndex = NoIndex;
        public int NormalIndex = NoIndex;
        public Vertex DuplicateVertex = null;
        public List<Vector3> Tangents = new List<Vector3>();
        public Vector3 AveragedTangent = new Vector3(0, 0, 0);
        public Vertex(int index,Vector3 position, VertexSkinData weightsData)
        {
            Index = index;
            Position = position;
            WeightsData = weightsData;
            Length = position.Length();
        }
        
        	
        public void AddTangent(Vector3 tangent)
        {
            Tangents.Add(tangent);
        }
	
        public void AverageTangents()
        {
            if(Tangents.Count == 0)
            {
                return;
            }

            foreach (var tangent in Tangents)
            {
                AveragedTangent += tangent;
            }

            AveragedTangent = Vector3.Normalize(AveragedTangent);
        }
	
        public Vector3 GetAverageTangent()
        {
            return AveragedTangent;
        }
        
        public bool IsSet(){
            return TextureIndex != NoIndex && NormalIndex != NoIndex;
        }
        
        public bool HasSameTextureAndNormal(int textureIndexOther, int normalIndexOther){
            return textureIndexOther == TextureIndex && normalIndexOther == NormalIndex;
        }
    }
}