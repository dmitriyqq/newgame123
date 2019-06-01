using System.Collections.Generic;

namespace GameRenderer
{
    public class CompoundMesh : Mesh
    {
        private List<Mesh> _children = new List<Mesh>();
        public CompoundMesh(Geometry geometry, Material material) : base(geometry, material)
        {
        }
        
        public override IEnumerable<Mesh> GetAllMeshes()
        {
            
            foreach (var child in _children)
            {
                yield return child;                
            }
            yield return this;
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

        public virtual void AddChild(Mesh mesh)
        {
            mesh.Parent = this;
            _children.Add(mesh);
        }
    }
}