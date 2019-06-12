using GameModel;
using GameRenderer.Animation.ColladaParser.DataStructures;
using GameRenderer.Materials;
using GameRenderer.OpenGL;
using OpenTK.Graphics.OpenGL4;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class AnimatedModelLoader
    {
	    private readonly Logger _logger;
	    private readonly ColladaLoader _modelLoader;
	    private readonly AnimationLoaderWrapper _animationLoader;
	    
	    public AnimatedModelLoader(Logger logger)
	    {
		    _logger = logger;
		    _modelLoader = new ColladaLoader(logger);
		    _animationLoader = new AnimationLoaderWrapper(_modelLoader);
	    }
	    
		public AnimatedModel LoadEntity(string modelFile, string textureFile, Material material) 
		{
			var entityData = _modelLoader.LoadColladaModel(modelFile, 3);
			var model = CreateVao(entityData.GetMeshData());
			var texture = LoadTexture(textureFile);
			var skeletonData = entityData.GetJointsData();
			var headJoint = CreateJoints(skeletonData.HeadJoint);

			var animatedModel = new AnimatedModel(model, texture, material, headJoint, skeletonData.JointCount);
			var animation = _animationLoader.LoadAnimation(modelFile);
			animatedModel.Animations.Add(animation);
			return animatedModel;
		}
	
		private static Texture LoadTexture(string textureFile) 
		{
			var diffuseTexture = new Texture(textureFile);
			return diffuseTexture;
		}
	
		private static Joint CreateJoints(JointData data) 
		{
			var joint = new Joint(data.index, data.nameId, data.bindLocalTransform);

			foreach (var child in data.children) 
			{
				joint.AddChild(CreateJoints(child));
			}

			return joint;
		}
	
		private VertexArray CreateVao(MeshData data) {
			var vao = new VertexArray(data.Vertices.Length, PrimitiveType.Triangles, _logger);
			vao.UseIndices(data.Indices);
			vao.AttachComponent( "vertices", BufferUsageHint.StaticDraw, data.Vertices, 3,  1);
			vao.AttachComponent("UV", BufferUsageHint.StaticDraw, data.TextureCoords,2, 1);
			vao.AttachComponent("normals", BufferUsageHint.StaticDraw, data.Normals,3,  1);
			vao.AttachIntegerComponent("joint ids", BufferUsageHint.StaticDraw, data.JointIds,3, 1);
			vao.AttachComponent("vertex weights", BufferUsageHint.StaticDraw, data.VertexWeights,3, 1);
			vao.GenerateVertexAttribPointer();
			return vao;
		}

    }
}