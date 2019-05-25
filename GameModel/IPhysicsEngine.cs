using System;

namespace GameModel
{
    public interface IPhysicsEngine
    {
        void Update(float deltaTime);

        void AddGameObject(GameObject gameObject);

        void RemoveGameObject(GameObject gameObject);
        
        event Action<GameObject, GameObject> OnCollision;
    }
}