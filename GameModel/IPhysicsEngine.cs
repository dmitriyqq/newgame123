using System;
using System.Numerics;
using BepuUtilities;

namespace GameModel
{
    public interface IPhysicsEngine
    {
        void Update(float deltaTime);
        void AddGameObject(GameObject gameObject);
        void RemoveGameObject(GameObject gameObject);
        RigidTransform GetTransform(int handle);
        event Action<GameObject, GameObject> OnCollision;
        Vector3? IntersectMap(Map map, Vector3 start, Vector3 dir);
        int? IntersectGameObjects(Vector3 start, Vector3 dir);
    }
}