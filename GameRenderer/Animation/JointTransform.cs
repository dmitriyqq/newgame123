using System.Numerics;
using GameModel;
using GameUtils;
using GlmNet;

namespace GameRenderer.Animation
{
	public class JointTransform
	{
		private readonly Vector3 Position;
		private readonly Quaternion Rotation;
		public JointTransform(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}

		internal mat4 GetLocalTransform()
		{
			var matrix = mat4.identity();
			glm.translate(matrix, Position.ToGlm());
			matrix = matrix * Rotation.ToRotationMatrix();
			return matrix;
		}

		internal static JointTransform Interpolate(JointTransform frameA, JointTransform frameB, float progression)
		{
			var pos = Interpolate(frameA.Position, frameB.Position, progression);
			var rot = Interpolate(frameA.Rotation, frameB.Rotation, progression);
			return new JointTransform(pos, rot);
		}

		private static Vector3 Interpolate(Vector3 start, Vector3 end, float progression)
		{
			var x = start.X + (end.X - start.X) * progression;
			var y = start.Y + (end.Y - start.Y) * progression;
			var z = start.Z + (end.Z - start.Z) * progression;
			return new Vector3(x, y, z);
		}
		
		private static Quaternion Interpolate(Quaternion a, Quaternion b, float blend)
		{
			var result = new Quaternion(0, 0, 0, 1);

			var dot = a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
			var blendI = 1f - blend;

			if (dot < 0) {
				result.W = blendI * a.W + blend * -b.W;
				result.X = blendI * a.X + blend * -b.X;
				result.Y = blendI * a.Y + blend * -b.Y;
				result.Z = blendI * a.Z + blend * -b.Z;
			} else {
				result.W = blendI * a.W + blend * b.W;
				result.X = blendI * a.X + blend * b.X;
				result.Y = blendI * a.Y + blend * b.Y;
				result.Z = blendI * a.Z + blend * b.Z;
			}

			Quaternion.Normalize(result);
			return result;
		}

	}
}