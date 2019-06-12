using System.Collections.Generic;
using Assimp;
using GameRenderer.Materials;
using GameRenderer.OpenGL;
using GlmNet;
using Material = GameRenderer.Materials.Material;

namespace GameRenderer.Animation
{
    public class AnimatedModel : IDrawable
    {
		private readonly VertexArray _vertexArray;
		private readonly Animator _animator;
		private readonly Texture _texture;
		public Joint RootJoint { get; }
		public int JointCount { get; }
		public IDrawable Parent { get; set; }
		public vec3 Position { get; set; }
		public vec3 Rotation { get; set; }
		public vec3 Scale { get; set; }
		public bool Visible { get; set; }
		private readonly Material _material;
		public readonly List<Animation> Animations = new List<Animation>();
		public AnimatedModel(VertexArray vertexArray, Texture texture, Material material, Joint rootJoint, int jointCount) 
		{
			_vertexArray = vertexArray;
			RootJoint = rootJoint;
			JointCount = jointCount;
			_animator = new Animator(this);
			_texture = texture;
			_material = material;
			rootJoint.CalcInverseBindTransform(mat4.identity());
		}
	
		public void DoAnimation(Animation animation) 
		{
			_animator.DoAnimation(animation);
		}
		
		public mat4 GetModelMatrix()
		{
			return mat4.identity();
		}

		public IEnumerable<Mesh> GetAllMeshes()
		{
			yield return new Mesh(_vertexArray, _material);
		}

		public void Update(float deltaTime) 
		{
			_animator.Update(deltaTime);
			_material.Uniform("jointTransforms", GetJointTransforms());
		}
	
		public mat4[] GetJointTransforms() 
		{
			var jointMatrices = new mat4[JointCount];
			AddJointsToArray(RootJoint, jointMatrices);
			return jointMatrices;
		}
	
		private static void AddJointsToArray(Joint headJoint, mat4[] jointMatrices) 
		{
			jointMatrices[headJoint.Index] = headJoint.AnimatedTransform;

			foreach (var childJoint in headJoint.Children) 
			{
				AddJointsToArray(childJoint, jointMatrices);
			}
		}
    }
}