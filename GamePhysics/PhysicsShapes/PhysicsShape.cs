using System.Xml.Serialization;

namespace GamePhysics.PhysicsShapes
{
    [XmlInclude(typeof(Box))]
    [XmlInclude(typeof(Sphere))]
    [XmlInclude(typeof(CompoundShape))]
    public abstract class PhysicsShape
    {
        public string Name { get; set; } 
    }
}