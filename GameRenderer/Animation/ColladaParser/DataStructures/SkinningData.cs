using System;
using System.Collections.Generic;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class SkinningData
    {
        public readonly List<string> jointOrder;
        public readonly List<VertexSkinData> verticesSkinData;
	
        public SkinningData(List<string> jointOrder, List<VertexSkinData> verticesSkinData)
        {
            this.jointOrder = jointOrder;
            this.verticesSkinData = verticesSkinData;
        }
    }
}