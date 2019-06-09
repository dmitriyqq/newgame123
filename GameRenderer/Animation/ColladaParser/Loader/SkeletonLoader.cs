using System;
using System.Collections.Generic;
using System.Xml;
using GameRenderer.Animation.ColladaParser.DataStructures;
using GlmNet;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class SkeletonLoader
    {
        private XmlNode _armatureData;
        private List<string> _boneOrder;
        private int _jointCount;
	
        private static readonly mat4 Correction = glm.rotate(mat4.identity(), - (float) Math.PI / 2, new vec3(1, 0, 0));
        
        public SkeletonLoader(XmlNode visualSceneNode, List<string> boneOrder) 
        {
            _boneOrder = boneOrder;
            _armatureData = visualSceneNode["visual_scene"].GetChildWithAttribute("node", "id", "Armature");
        }
	
        public SkeletonData ExtractBoneData()
        {
            XmlNode headNode = _armatureData["node"];
            var headJoint = LoadJointData(headNode, true);
            return new SkeletonData(_jointCount, headJoint);
        }
	
        private JointData LoadJointData(XmlNode jointNode, bool isRoot)
        {
            var joint = ExtractMainJointData(jointNode, isRoot);
            foreach (var childNode in jointNode.GetChildren("node"))
            {
                joint.AddChild(LoadJointData(childNode, false));
            }
            return joint;
        }
	
        private JointData ExtractMainJointData(XmlNode jointNode, bool isRoot){
            if (jointNode.Attributes == null)
            {
                return null;
            }
            
            var nameId = jointNode.Attributes["id"].Value;
            var index = _boneOrder.FindIndex(x => x.Equals(nameId));
                
            var matrixData = jointNode.GetChild("matrix").InnerText.Split(' ');
            var matrix = ConvertData(matrixData);
            matrix = Transpose(matrix);                    
            if(isRoot) {
                //because in Blender z is up, but in our game y is up.
                matrix = Correction * matrix;
            }

            _jointCount++;
            return new JointData(index, nameId, matrix);

        }

        private static mat4 Transpose(mat4 mm)
        {
            // glm net doesn't have transpose function???? 
            var m = mat4.identity();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    m[i, j] = mm[j, i];
                }
            }

            return m;
        }
	
        private static mat4 ConvertData(string[] rawData)
        {
            var matrixData = mat4.identity();
            for (var i = 0; i < 15; i++)
            {
                matrixData[i / 4, i % 4] = float.Parse(rawData[i]);
            }
            
            return matrixData;
        }
    }
}