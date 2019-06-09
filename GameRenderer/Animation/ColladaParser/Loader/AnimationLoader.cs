using System;
using System.Xml;
using GameRenderer.Animation.ColladaParser.DataStructures;
using GlmNet;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class AnimationLoader
    {
	    private static readonly mat4 Correction = glm.rotate(mat4.identity(), - (float) Math.PI / 2, new vec3(1, 0, 0));
	
		private XmlNode animationData;
		private XmlNode jointHierarchy;

		private static readonly char[] Separator = {' '};
		
		public AnimationLoader(XmlNode animationData, XmlNode jointHierarchy)
		{
			this.animationData = animationData;
			this.jointHierarchy = jointHierarchy;
		}
		
		public AnimationData ExtractAnimation()
		{
			var rootNode = FindRootJointName();
			var times = GetKeyTimes();
			var duration = times[times.Length - 1];
			var keyFrames = InitKeyFrames(times);
			var animationNodes = animationData.GetChildren("animation");
			foreach(var jointNode in animationNodes)
			{
				LoadJointTransforms(keyFrames, jointNode, rootNode);
			}
			return new AnimationData(duration, keyFrames);
		}
		
		private float[] GetKeyTimes()
		{
			var timeData = animationData.GetChild("animation").GetChild("source").GetChild("float_array");
			var rawTimes = timeData.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			var times = new float[rawTimes.Length];

			for(var i = 0; i < times.Length; i++)
			{
				times[i] = float.Parse(rawTimes[i]);
			}
			return times;
		}
		
		private KeyFrameData[] InitKeyFrames(float[] times)
		{
			var frames = new KeyFrameData[times.Length];

			for(var i=0;i<frames.Length;i++)
			{
				frames[i] = new KeyFrameData(times[i]);
			}
			return frames;
		}
		
		private void LoadJointTransforms(KeyFrameData[] frames, XmlNode jointData, string rootNodeId)
		{
			var jointNameId = GetJointName(jointData);
			var dataId = GetDataId(jointData);
			var transformData = jointData.GetChildWithAttribute("source", "id", dataId);
			var rawData = transformData.GetChild("float_array").InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			ProcessTransforms(jointNameId, rawData, frames, jointNameId.Equals(rootNodeId));
		}
		
		private static string GetDataId(XmlNode jointData)
		{
			var node = jointData.GetChild("sampler").GetChildWithAttribute("input", "semantic", "OUTPUT");
			return node?.Attributes?["source"]?.Value.Substring(1);
		}
		
		private static string GetJointName(XmlNode jointData)
		{
			var channelNode = jointData.GetChild("channel");
			var data = channelNode.Attributes?["target"].Value;
			return data?.Split('/')[0];
		}
		
		private static void ProcessTransforms(string jointName, string[] rawData, KeyFrameData[] keyFrames, bool root)
		{
			for(var i = 0; i < keyFrames.Length; i++)
			{
				var transform = mat4.identity();
				for (var j = 0; j < 16; j++)
				{
					// order of indices is inverse as we need to transpose matrix
					transform[j % 4, j / 4] = float.Parse(rawData[i*16 + j]);
				}

				if (root)
				{
					//because up axis in Blender is different to up axis in game
					transform = Correction * transform;
				}
				keyFrames[i].AddJointTransform(new JointTransformData(jointName, transform));
			}
		}
		
		private string FindRootJointName()
		{
			var skeleton = jointHierarchy.GetChild("visual_scene").GetChildWithAttribute("node", "id", "Armature");
			return skeleton.GetChild("node")?.Attributes?["id"]?.Value;
		}

    }
}