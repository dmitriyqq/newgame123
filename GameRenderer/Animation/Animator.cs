using System;
using System.Collections.Generic;
using GlmNet;

namespace GameRenderer.Animation
{
    public class Animator
    {
        private readonly AnimatedModel _entity;
		private Animation _currentAnimation;
		private float _animationTime;

		public Animator(AnimatedModel entity) {
			_entity = entity;
		}
	
		public void DoAnimation(Animation animation) {
			_animationTime = 0;
			_currentAnimation = animation;
		}
	
		public void Update(float deltaTime) {
			if (_currentAnimation == null) 
			{
				return;
			}

			IncreaseAnimationTime(deltaTime);
			var currentPose = CalculateCurrentAnimationPose();
			ApplyPoseToJoints(currentPose, _entity.RootJoint, mat4.identity());
		}
	
		private void IncreaseAnimationTime(float deltaTime) {
			_animationTime += deltaTime;
			if (_animationTime > _currentAnimation.Length)
			{
				_animationTime %= _currentAnimation.Length;
			}
		}
	
		private Dictionary<string, mat4> CalculateCurrentAnimationPose() {
			var frames = GetPreviousAndNextFrames();
			var progression = CalculateProgression(frames[0], frames[1]);
			return InterpolatePoses(frames[0], frames[1], progression);
		}
		private KeyFrame[] GetPreviousAndNextFrames() {
			var allFrames = _currentAnimation.KeyFrames;
			var previousFrame = allFrames[0];
			var nextFrame = allFrames[0];
			
			for (var i = 1; i < allFrames.Length; i++) {
				nextFrame = allFrames[i];

				if (nextFrame.TimeStamp > _animationTime) {
					break;
				}

				previousFrame = allFrames[i];
			}
			return new [] { previousFrame, nextFrame };
		}
	
		private float CalculateProgression(KeyFrame previousFrame, KeyFrame nextFrame) {
			var totalTime = nextFrame.TimeStamp - previousFrame.TimeStamp;
			var currentTime = _animationTime - previousFrame.TimeStamp;
			return currentTime / totalTime;
		}

		private static Dictionary<string, mat4> InterpolatePoses(KeyFrame previousFrame, KeyFrame nextFrame, float progression) {
			var currentPose = new Dictionary<string, mat4>();

			foreach (var joint in previousFrame.Pose)
			{
				var jointName = joint.Key;
				var previousTransform = previousFrame.Pose[jointName];
				var nextTransform = nextFrame.Pose[jointName];
				var currentTransform = JointTransform.Interpolate(previousTransform, nextTransform, progression);
				currentPose[jointName] = currentTransform.GetLocalTransform();
			}
			return currentPose;
		}
		
		private static void ApplyPoseToJoints(IReadOnlyDictionary<string, mat4> currentPose, Joint joint, mat4 parentTransform) {
			var currentLocalTransform = currentPose[joint.Name];
			var currentTransform = parentTransform * currentLocalTransform;

			foreach (var childJoint in joint.Children) 
			{
				ApplyPoseToJoints(currentPose, childJoint, currentTransform);
			}

			currentTransform =  currentTransform * joint.InverseBindTransform;
			joint.AnimatedTransform = currentTransform;
		}
    }
}