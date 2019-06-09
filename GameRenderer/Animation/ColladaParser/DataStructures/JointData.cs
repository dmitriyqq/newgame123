using System.Collections.Generic;
using GlmNet;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class JointData
    {
        public readonly int index;
        public readonly string nameId;
        public readonly mat4 bindLocalTransform;
        public readonly List<JointData> children = new List<JointData>();

        public JointData(int index, string nameId, mat4 bindLocalTransform)
        {
            this.index = index;
            this.nameId = nameId;
            this.bindLocalTransform = bindLocalTransform;
        }

        public void AddChild(JointData child) 
        {
            children.Add(child);
        }
    }
}