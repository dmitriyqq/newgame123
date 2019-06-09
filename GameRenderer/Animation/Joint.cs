using System;
using System.Collections.Generic;
using GlmNet;

namespace GameRenderer.Animation
{
    public class Joint
    {
		public int Index;// ID
		public string Name;
		public List<Joint> Children = new List<Joint>();
	
		public mat4 AnimatedTransform { get; set; } = mat4.identity();
		public mat4 InverseBindTransform { get; private set; }
		private mat4 _localBindTransform;
	
		public Joint(int index, string name, mat4 bindLocalTransform) {
			Index = index;
			Name = name;
			_localBindTransform = bindLocalTransform;
		}
	
		public void AddChild(Joint child) {
			Children.Add(child);
		}

		public void CalcInverseBindTransform(mat4 parentBindTransform) {
			var bindTransform = parentBindTransform * _localBindTransform;
			InverseBindTransform = glm.inverse(bindTransform);

			foreach (var child in Children)
			{
				child.CalcInverseBindTransform(bindTransform);
			}
		}
    }
}