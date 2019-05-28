using System.Numerics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Trees;
using GameModel;
using GameModel.GameObjects;

namespace GamePhysics
{
    public class MapHitHandler : IRayHitHandler
    {
        private readonly Map _map;

        public MapHitHandler(Map map)
        {
            _map = map;
        }
        
        public bool AllowTest(CollidableReference collidable)
        {
            return collidable.Handle == _map.Handle;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable)
        {
            if (t < maximumT)
            {
                maximumT = t;
            }

            Collision = ray.Origin + ray.Direction * t;
        }

        public Vector3? Collision = null;
    }
}