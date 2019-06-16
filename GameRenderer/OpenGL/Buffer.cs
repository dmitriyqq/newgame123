using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GameModel;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer.OpenGL
{
    public class Buffer
    {
        protected readonly int Id;
        protected readonly Logger Logger;
        protected readonly BufferTarget Target;
        protected readonly BufferUsageHint Hint;
        public readonly string Name;
        public VertexAttribPointerType Type { get; }
        public VertexAttribIntegerType IType { get; }
        public int TypeSize { get; } 
        public int ElementsCount { get; }
        
        public int StrideSize => ElementsCount * TypeSize;
        protected Buffer(string name, BufferTarget target, BufferUsageHint hint, int elementsCount, int typeSize, VertexAttribPointerType type, VertexAttribIntegerType itype, Logger logger)
        {
            Target = target;
            Hint = hint;
            Name = name;
            Id = GL.GenBuffer();
            Type = type;
            TypeSize = typeSize;
            ElementsCount = elementsCount;
            IType = itype;
            Logger = logger;
            Logger.Info($"Created buffer id={Id}, target = {Target}, Hint = {Hint}, TypeSize={TypeSize}, ElementsCount={ElementsCount}, Type={Type}");
        }

        public void Bind()
        {
            Logger.Info($"Bind buffer id={Id}");
            GL.BindBuffer(Target, Id);
        }

        ~Buffer()
        {
            Logger.Info($"Delete buffer id={Id}");
            GL.DeleteBuffer(Id);
        }
    }

    public class Buffer<T>: Buffer where T : struct
    {
        private bool _bufferedData;

        internal Buffer(string name, BufferTarget target, BufferUsageHint hint, int elementsCount, VertexAttribPointerType type, VertexAttribIntegerType itype, Logger logger)
            : base(name, target, hint, elementsCount, Unsafe.SizeOf<T>(), type, itype, logger)
        {
        }

        public void BufferData(T[] data)
        {
            Bind();

            if (_bufferedData)
            {
                Logger.Info($"Update data for buffer {Id}, new data = {data.Length}, in bytes = {data.Length * TypeSize}");
                GL.BufferSubData(Target, IntPtr.Zero, data.Length * TypeSize, data);
            }
            else
            {
                Logger.Info($"Buffer data for buffer {Id}, new data = {data.Length}, in bytes = {data.Length * TypeSize}");
                GL.BufferData(Target, data.Length * TypeSize, data, Hint);
                _bufferedData = true;
            }
        }
    }
}