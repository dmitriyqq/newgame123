using System.Numerics;
using BepuPhysics;
using GameModel;

namespace PhysicsEngine
{
    public class BodyAdaptor : IPhysicsBody
    {
        private BodyDescription bd;
        public BodyAdaptor(BodyDescription bd)
        {
            this.bd = bd;
        }

        public Vector3 Position => bd.Pose.Position;

        public int Handle;
    }
}