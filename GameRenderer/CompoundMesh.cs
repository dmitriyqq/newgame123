using System.Collections.Generic;
using GameRenderer.Metadata;

namespace GameRenderer
{
    public class CompoundMesh : Mesh
    {
        private List<Mesh> _children = new List<Mesh>();

        public CompoundMesh() : base()
        {
            
        }
        
        public override IEnumerable<Mesh> GetAllMeshes()
        {
            yield return this;

            foreach (var child in _children)
            {
                yield return child;                
            }
        }
        public override Mesh Clone()
        {
            var mesh = MemberwiseClone() as CompoundMesh;
            
            mesh._children = new List<Mesh>();
            foreach (var child in _children)
            {
                    mesh._children.Add(child.Clone());
            }

            return mesh;
        }

        public void AddChild(Mesh mesh)
        {
            mesh.Parent = this;
            _children.Add(mesh);
        }
    }
}