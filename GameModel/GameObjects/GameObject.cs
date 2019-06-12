using System;
using System.Xml.Serialization;
using BepuUtilities;
using Quaternion = BepuUtilities.Quaternion;

namespace GameModel.GameObjects
{
    [Serializable]
    [XmlInclude(typeof(Map))]
    [XmlInclude(typeof(ArmyGameObject))]
    public class GameObject
    {
        public RigidTransform Transform;
        
        public int Handle;
        public bool Static;

        public string Asset;
        public string Player { get; set; }
        
        [XmlIgnore]
        public Model Model;

        public GameObject()
        {
            Transform = new RigidTransform {Orientation = Quaternion.Identity};
        }

        public virtual void Update(float deltaTime)
        {
            Transform = Model.Engine.GetTransform(Handle);
        }
    }
}