using System;
using System.Collections.Generic;
using GameModel;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.OpenGL
{
    public class VertexArray : Geometry
    {
        private readonly int _id;
        private readonly List<Buffer> _buffers = new List<Buffer>();
        private readonly List<int> _multiplicity = new List<int>();
        private readonly int _vertexCount;
        private readonly PrimitiveType _type;
        private readonly Logger _logger;
        private int _elementsCount;
        private Buffer<int> _indices;

        public VertexArray(int vertexCount, PrimitiveType type, Logger logger)
        {
            _vertexCount = vertexCount;
            _type = type;
            _id = GL.GenVertexArray();
            _logger = logger;
            _logger.Info($"Create Vertex Array vertexCount: {vertexCount}, type: {type}, id: {_id}");
        }

        ~VertexArray()
        {
//            _logger.Info($"Destroying Vertex array id: {_id}");
//            GL.DeleteVertexArray(_id);
        }
        
        private void Bind()
        {
            GL.BindVertexArray(_id);
        }

        public void GenerateVertexAttribPointer()
        {
            Bind();
            var index = 0;
            foreach (var buffer in _buffers)
            {
                buffer.Bind();
                GL.EnableVertexAttribArray(index);
                switch (buffer.Type)
                {
                    case VertexAttribPointerType.Int:
                        _logger.Info("Int VertexAttribPointer ");
                        GL.VertexAttribIPointer(index, buffer.ElementsCount, buffer.IType, 0, IntPtr.Zero);
                        break;
                    case VertexAttribPointerType.Float:
                        _logger.Info("Float VertexAttribPointer ");
                        GL.VertexAttribPointer(index, buffer.ElementsCount, buffer.Type, false, 0, 0);
                        break;
                    default:
                        throw new NotImplementedException("Only float and integers supported");
                }

                // GL.VertexAttribDivisor(index, _multiplicity[index]);
                index++;
            }
            _logger.Info($"Created Vertex Attrib pointer for VAO id: {_id}, buffers: {index}");
        }

        public Buffer<float> AttachComponent(string name, BufferUsageHint hint, float[] data, int elementsCount, VertexAttribPointerType type, int divisor)
        {
            Bind();
            var buffer = new Buffer<float>(name, BufferTarget.ArrayBuffer, hint, elementsCount, type, VertexAttribIntegerType.Int, _logger);
            buffer.BufferData(data);
            AttachVbo(buffer, divisor);
            return buffer;
        }

        public Buffer AttachIntegerComponent(string name, BufferUsageHint hint, int[] data, int elementsCount, VertexAttribIntegerType type, int divisor)
        {
            Bind();
            var buffer = new Buffer<int>(name, BufferTarget.ArrayBuffer, hint, elementsCount, VertexAttribPointerType.Int, type, _logger);
            buffer.BufferData(data);
            AttachVbo(buffer, divisor);
            return buffer;
        }
        
        private void AttachVbo(Buffer buffer, int divisor)
        {
            Bind();
            _buffers.Add(buffer);
            _multiplicity.Add(divisor);
        }

        public void UseIndices(int[] indices)
        {
            Bind();
            var buffer = new Buffer<int>("indices", BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw, 0, VertexAttribPointerType.UnsignedInt, VertexAttribIntegerType.UnsignedInt, _logger);
            buffer.BufferData(indices);

            _elementsCount = indices.Length;
            _indices = buffer;
            _indices.Bind();
        }

        public override void Draw()
        {
            Bind();
            
            if (_indices == null)
            {
                GL.DrawArrays(_type, 0, _vertexCount);
            }
            else
            {
                DrawElements();
            }
        }

        private void DrawElements()
        {
            Bind();
            try
            {
                GL.DrawElements(_type, _elementsCount, DrawElementsType.UnsignedInt, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}