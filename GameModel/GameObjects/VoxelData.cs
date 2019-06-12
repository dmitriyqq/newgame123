using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GameModel.GameObjects
{
    public class VoxelData : IXmlSerializable
    {
        private float[,] _data;

        public void InitializedData(int size)
        {
            _data = new float[size, size];
        }
        
        public float this[int i, int j]
        {
            get => _data[i, j];
            set => _data[i, j] = value;
        }
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(List<float>));
            var list = serializer.Deserialize(reader);

            if (list is List<float> vertexList)
            {
                CreateDataFromList(vertexList);    
            }
            else
            {
                throw new Exception("Couldn't deserialize map data");
            } 
        }

        public void WriteXml(XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(List<float>));
            serializer.Serialize(writer, GetVerticesFromData());
        }

        private List<float> GetVerticesFromData()
        {
            return _data.Cast<float>().ToList();
        }

        private void CreateDataFromList(IReadOnlyList<float> list)
        {
            var size = (int) Math.Sqrt(list.Count);

            if (size * size != list.Count)
            {
                throw new XmlException($"Couldn't create {nameof(VoxelData)} from List. List.Count should be square");
            }
            
            _data = new float[size, size];

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    _data[i, j] = list[i * size + j];
                }
            }
        }
    }
}