using System;
using System.Numerics;
using BepuUtilities;
using GameModel.GameObjects;

namespace GameModel
{
    public interface IPhysicsEngine
    {
        void Update(float deltaTime);
        void AddGameObject(GameObject gameObject);
        void RemoveGameObject(GameObject gameObject);
        RigidTransform GetTransform(int handle);
        event Action<GameObject, GameObject> OnCollision;
        Vector3? IntersectMap(Map map, Vector3 start, Vector3 dir, out Vector3 normal);
        int? IntersectGameObjects(Vector3 start, Vector3 dir);
    }
}