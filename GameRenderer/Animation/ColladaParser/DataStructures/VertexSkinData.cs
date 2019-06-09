using System;
using System.Collections.Generic;

namespace GameRenderer.Animation.ColladaParser.DataStructures
{
    public class VertexSkinData
    {
        public readonly List<int> JointIds = new List<int>();
        public readonly List<float> Weights = new List<float>();
	
        public void AddJointEffect(int jointId, float weight)
        {
            for (var i = 0; i < Weights.Count; i++)
            {
                if (weight > Weights[i])
                {
                    JointIds[i] = jointId;
                    Weights[i] = weight;
                    return;
                }
            }

            JointIds.Add(jointId);
            Weights.Add(weight);
        }
	
        public void LimitJointNumber(int max)
        {
            if(JointIds.Count > max)
            {
                var topWeights = new float[max];
                var total = SaveTopWeights(topWeights);
                RefillWeightList(topWeights, total);
                RemoveExcessJointIds(max);
            }
            else if(JointIds.Count < max)
            {
                FillEmptyWeights(max);
            }
        }

        private void FillEmptyWeights(int max)
        {
            while(JointIds.Count < max){
                JointIds.Add(0);
                Weights.Add(0f);
            }
        }
	
        private float SaveTopWeights(IList<float> topWeightsArray)
        {
            float total = 0;

            for(var i=0;i<topWeightsArray.Count; i++)
            {
                topWeightsArray[i] = Weights[i];
                total += topWeightsArray[i];
            }

            return total;
        }
	
        private void RefillWeightList(IEnumerable<float> topWeights, float total)
        {
            Weights.Clear();
            foreach (var t in topWeights)
            {
                Weights.Add(Math.Min(t/total, 1));
            }
        }
	
        private void RemoveExcessJointIds(int max)
        {
            while(JointIds.Count > max)
            {
                JointIds.RemoveAt(JointIds.Count - 1);
            }
        }
    }
}