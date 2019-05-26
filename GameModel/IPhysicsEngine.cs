using System;
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
    }
}