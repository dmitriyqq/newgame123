using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GameModel;
using GameRenderer.Animation.ColladaParser.DataStructures;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class SkinLoader
    {
	    private static readonly char[] Separator = {' '};
        private readonly XmlNode _skinningData;
		private readonly int _maxWeights;
		private readonly Logger _logger;
		public SkinLoader(XmlNode controllersNode, int maxWeights, Logger logger)
		{
			_logger = logger;

			try
			{
				_logger.Info("loading skin data");
				_skinningData = controllersNode["controller"]?["skin"];
				_maxWeights = maxWeights;
			}
			catch (Exception e)
			{
				logger.Error(e);
			}
		}
	
		public SkinningData ExtractSkinData() {
			try
			{
				var jointsList = LoadJointsList();
				var weights = LoadWeights();
				XmlNode weightsDataNode = _skinningData["vertex_weights"];
				var effectorJointCounts = GetEffectiveJointsCounts(weightsDataNode);
				var vertexWeights = GetSkinData(weightsDataNode, effectorJointCounts, weights);
				return new SkinningData(jointsList, vertexWeights);
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}

			return null;

		}
	
		private List<string> LoadJointsList() 
		{
			XmlNode inputNode = _skinningData["vertex_weights"];

			var xmlAttributeCollection = inputNode.GetChildWithAttribute("input", "semantic", "JOINT").Attributes;
			if (xmlAttributeCollection == null)
			{
				return null;
			}

			var jointDataId = xmlAttributeCollection["source"].Value.Substring(1);
			XmlNode jointsNode = _skinningData.GetChildWithAttribute("source", "id", jointDataId)["Name_array"];

			var names = jointsNode?.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

			return names?.ToList();
		}
	
		private float[] LoadWeights()
		{
			XmlNode inputNode = _skinningData["vertex_weights"];
			var xmlAttributeCollection = inputNode.GetChildWithAttribute("input", "semantic", "WEIGHT").Attributes;
			if (xmlAttributeCollection == null)
			{
				return null;
			}

			var weightsDataId = xmlAttributeCollection["source"].Value.Substring(1);
			XmlNode weightsNode = _skinningData.GetChildWithAttribute("source", "id", weightsDataId)["float_array"];
			if (weightsNode == null)
			{
				return null;
			}

			var rawData = weightsNode.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			var weights = new float[rawData.Length];
			for (var i = 0; i < weights.Length; i++) 
			{
				weights[i] = float.Parse(rawData[i]);
			}
			return weights;
		}
	
		private static int[] GetEffectiveJointsCounts(XmlNode weightsDataNode) {
			var rawData = weightsDataNode["vcount"]?.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			if (rawData == null)
			{
				return null;
			}

			var counts = new int[rawData.Length];

			for (var i = 0; i < rawData.Length; i++) 
			{
				counts[i] = int.Parse(rawData[i]);
			}
			return counts;

		}
	
		private List<VertexSkinData> GetSkinData(XmlNode weightsDataNode, IEnumerable<int> counts, IReadOnlyList<float> weights) 
		{
			var rawData = weightsDataNode["v"]?.InnerXml.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			var skinningData = new List<VertexSkinData>();
			var pointer = 0;

			try
			{
				foreach (var count in counts)
				{
					var skinData = new VertexSkinData();
					for (var i = 0; i < count; i++)
					{
						var jointId = int.Parse(rawData[pointer++]);
						var weightId = int.Parse(rawData[pointer++]);
						if (weightId < weights.Count)
						{
							skinData.AddJointEffect(jointId, weights[weightId]);
						}
						else
						{
							throw new IndexOutOfRangeException();
						}
					}

					skinData.LimitJointNumber(_maxWeights);
					skinningData.Add(skinData);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}

			return skinningData;
		}

    }
}