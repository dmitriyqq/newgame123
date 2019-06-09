using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using GameRenderer.Animation.ColladaParser.DataStructures;
using GlmNet;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public class GeometryLoader
    {
        
		private static readonly mat4 Correction = glm.rotate(mat4.identity(), - (float) Math.PI / 2, new vec3(1, 0, 0));
		
		private readonly XmlNode _meshData;
		private readonly char[] Separator = {' '};
		private readonly List<VertexSkinData> _vertexWeights;
		
		private float[] _verticesArray;
		private float[] _normalsArray;
		private float[] _texturesArray;
		private int[] _indicesArray;
		private int[] _jointIdsArray;
		private float[] _weightsArray;
	
		private readonly List<Vertex> _vertices = new List<Vertex>();
		private readonly List<Vector2> _textures = new List<Vector2>();
		private readonly List<Vector3> _normals = new List<Vector3>();
		private readonly List<int> _indices = new List<int>();
		
		public GeometryLoader(XmlNode geometryNode, List<VertexSkinData> vertexWeights) {
			_vertexWeights = vertexWeights;
			_meshData = geometryNode.GetChild("geometry").GetChild("mesh");
		}
		
		public MeshData ExtractModelData(){
			ReadRawData();
			AssembleVertices();
			RemoveUnusedVertices();
			InitArrays();
			ConvertDataToArrays();
			ConvertIndicesListToArray();
			return new MeshData(_verticesArray, _texturesArray, _normalsArray, _indicesArray, _jointIdsArray, _weightsArray);
		}
	
		private void ReadRawData() {
			ReadPositions();
			ReadNormals();
			ReadTextureCoords();
		}
	
		private void ReadPositions()
		{
			var xmlAttributeCollection = _meshData.GetChild("vertices").GetChild("input").Attributes;
			if (xmlAttributeCollection == null)
			{
				return;
			}

			var positionsId = xmlAttributeCollection["source"].Value.Substring(1);
			var positionsData = _meshData.GetChildWithAttribute("source", "id", positionsId).GetChild("float_array");
			if (positionsData.Attributes == null)
			{
				return;
			}

			var count = int.Parse(positionsData.Attributes["count"].Value);
			var posData = positionsData.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < count/3; i++) {
				var x = float.Parse(posData[i * 3]);
				var y = float.Parse(posData[i * 3 + 1]);
				var z = float.Parse(posData[i * 3 + 2]);
				var position = new vec4(x, y, z, 1);
				position = Correction * position;
				_vertices.Add(new Vertex(_vertices.Count, new Vector3(position.x, position.y, position.z), _vertexWeights[_vertices.Count]));
			}
		}
	
		private void ReadNormals()
		{
			var polylist = _meshData.GetChild("polylist");

			var xmlAttributeCollection = polylist?.GetChildWithAttribute("input", "semantic", "NORMAL")?.Attributes;

			if (xmlAttributeCollection == null)
			{
				return;
			}

			var normalsId = xmlAttributeCollection["source"].Value.Substring(1);
			var normalsData = _meshData.GetChildWithAttribute("source", "id", normalsId).GetChild("float_array");
			if (normalsData.Attributes == null)
			{
				return;
			}

			var count = int.Parse(normalsData.Attributes["count"].Value);
			var normData = normalsData.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < count/3; i++) {
				var x = float.Parse(normData[i * 3]);
				var y = float.Parse(normData[i * 3 + 1]);
				var z = float.Parse(normData[i * 3 + 2]);
				var norm = new vec4(x, y, z, 0f);
				norm = Correction * norm;
				_normals.Add(new Vector3(norm.x, norm.y, norm.z));
			}
		}
	
		private void ReadTextureCoords()
		{
			var xmlAttributeCollection = _meshData.GetChild("polylist").GetChildWithAttribute("input", "semantic", "TEXCOORD")
				.Attributes;
			if (xmlAttributeCollection == null)
			{
				return;
			}

			var texCoordsId = xmlAttributeCollection["source"].Value.Substring(1);
			var texCoordsData = _meshData.GetChildWithAttribute("source", "id", texCoordsId).GetChild("float_array");
			if (texCoordsData.Attributes == null)
			{
				return;
			}
			
			var count = int.Parse(texCoordsData.Attributes["count"].Value);
			var texData = texCoordsData.InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < count/2; i++) {
				var s = float.Parse(texData[i * 2]);
				var t = float.Parse(texData[i * 2 + 1]);
				_textures.Add(new Vector2(s, t));
			}
		}
		
		private void AssembleVertices()
		{
			var poly = _meshData.GetChild("polylist");
			var typeCount = poly.GetChildren("input").Count;
			var indexData = poly.GetChild("p").InnerText.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			for(var i = 0; i < indexData.Length / typeCount; i++){
				var positionIndex = int.Parse(indexData[i * typeCount]);
				var normalIndex = int.Parse(indexData[i * typeCount + 1]);
				var texCoordIndex = int.Parse(indexData[i * typeCount + 2]);
				ProcessVertex(positionIndex, normalIndex, texCoordIndex);
			}
		}
		
	
		private Vertex ProcessVertex(int posIndex, int normIndex, int texIndex) 
		{
			var currentVertex = _vertices[posIndex];
			if (currentVertex.IsSet())
			{
				return DealWithAlreadyProcessedVertex(currentVertex, texIndex, normIndex);
			}

			currentVertex.TextureIndex = texIndex;
			currentVertex.NormalIndex = normIndex;
			_indices.Add(posIndex);
			return currentVertex;

		}
	
		private int[] ConvertIndicesListToArray()
		{
			_indicesArray = new int[_indices.Count];
			for (var i = 0; i < _indicesArray.Length; i++)
			{
				_indicesArray[i] = _indices[i];
			}
			return _indicesArray;
		}
	
		private float ConvertDataToArrays() 
		{
			var furthestPoint = 0.0f;
			for (var i = 0; i < _vertices.Count; i++) 
			{
				var currentVertex = _vertices[i];
				if (currentVertex.Length > furthestPoint) 
				{
					furthestPoint = currentVertex.Length;
				}

				var position = currentVertex.Position;
				var textureCoord = _textures[currentVertex.TextureIndex];
				var normalVector = _normals[currentVertex.NormalIndex];
				_verticesArray[i * 3] = position.X;
				_verticesArray[i * 3 + 1] = position.Y;
				_verticesArray[i * 3 + 2] = position.Z;
				_texturesArray[i * 2] = textureCoord.X;
				_texturesArray[i * 2 + 1] = 1 - textureCoord.Y;
				_normalsArray[i * 3] = normalVector.X;
				_normalsArray[i * 3 + 1] = normalVector.Y;
				_normalsArray[i * 3 + 2] = normalVector.Z;

				var weights = currentVertex.WeightsData;
				_jointIdsArray[i * 3] = weights.JointIds[0];
				_jointIdsArray[i * 3 + 1] = weights.JointIds[1];
				_jointIdsArray[i * 3 + 2] = weights.JointIds[2];
				_weightsArray[i * 3] = weights.Weights[0];
				_weightsArray[i * 3 + 1] = weights.Weights[1];
				_weightsArray[i * 3 + 2] = weights.Weights[2];
	
			}
			return furthestPoint;
		}
	
		private Vertex DealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex, int newNormalIndex) 
		{
			if (previousVertex.HasSameTextureAndNormal(newTextureIndex, newNormalIndex)) 
			{
				_indices.Add(previousVertex.Index);
				return previousVertex;
			}

			var anotherVertex = previousVertex.DuplicateVertex;

			if (anotherVertex != null) {
				return DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex);
			}

			var duplicateVertex = new Vertex(_vertices.Count, previousVertex.Position, previousVertex.WeightsData)
			{
				TextureIndex = newTextureIndex, NormalIndex = newNormalIndex
			};

			previousVertex.DuplicateVertex = duplicateVertex;

			_vertices.Add(duplicateVertex);
			_indices.Add(duplicateVertex.Index);

			return duplicateVertex;
		}
		
		private void InitArrays()
		{
			_verticesArray = new float[_vertices.Count * 3];
			_texturesArray = new float[_vertices.Count * 2];
			_normalsArray = new float[_vertices.Count * 3];
			_jointIdsArray = new int[_vertices.Count * 3];
			_weightsArray = new float[_vertices.Count * 3];
		}
	
		private void RemoveUnusedVertices() 
		{
			foreach (var vertex in _vertices) {
				vertex.AverageTangents();
				if (!vertex.IsSet()) {
					vertex.TextureIndex = 0;
					vertex.NormalIndex = 0;
				}
			}
		}
    }
}