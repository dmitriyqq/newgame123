using System;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using GameModel.GameObjects;
using GameModel.Tasks;
using GameRenderer.Materials;
using GameRenderer.Metadata;
using GlmNet;

namespace GameRenderer
{
    public class UnitMesh : IDrawable
    {
        private readonly IDrawable _drawable;
        private readonly List<Mesh> _children = new List<Mesh>();
        public readonly GameObject GameObject;
        public IDrawable Parent { get; set; }
        public bool Visible { get => _drawable.Visible; set => _drawable.Visible = value; }

        public UnitMesh(IDrawable drawable, GameObject gameObject)
        {
            _drawable = drawable;
            GameObject = gameObject;

            foreach (var child in _children)
            {
                child.Visible = false;
            }

            _drawable.Parent = this;
        }

        public vec3 Position
        {
            get => GameObject.Transform.Position.ToGlm();
            set {}
        }

        public vec3 Rotation
        {
            get => new vec3(1.0f, 0.0f, 0.0f); 
            set {}
        }

        public vec3 Scale { get => _drawable.Scale;
            set => _drawable.Scale = value;
        }
        
        public mat4 GetModelMatrix()
        {
            var m = glm.inverse(glm.lookAt(Position, Position + 30.0f * Rotation, new vec3(0.0f, 1.0f, 0.0f)));
            m = glm.rotate(m, (float)- Math.PI / 2.0f, new vec3(0.0f, 1.0f, 0.0f));
            m = glm.scale(m, Scale);
            return m;
        }

        public void Update(float deltaTime)
        {
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            var l = _drawable.GetAllMeshes();
                l = l.Concat(_children);

            return l;
        }
    }
}