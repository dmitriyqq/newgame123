using System;

namespace GameModel
{
    public interface IPhysicsEngine
    {
        void Update(float deltaTime);

        void AddUnit(Unit unit);

        void RemoveUnit(Unit unit);
        
        event Action<Unit, Unit> OnCollision;
    }
}