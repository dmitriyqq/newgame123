using System.Numerics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Trees;

namespace GamePhysics
{
    public class HitHandler : IRayHitHandler
    {
        public bool AllowTest(CollidableReference collidable)
        {
            return true;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable)
        {
            if (t < maximumT)
            {
                maximumT = t;
            }

            CollidableReference = collidable;
        }

        public CollidableReference? CollidableReference = null;
    }
}