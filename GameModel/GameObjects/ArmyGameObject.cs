using System.Linq;
using System.Numerics;
using GameModel.Tasks;

namespace GameModel.GameObjects
{
    public class ArmyGameObject : GameObject
    {

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

//            if (CurrentTask is Move move)
//            {
//                moveTo(move.Target, 1.0f, deltaTime);
//            }
//
//            if (CurrentTask is Follow follow)
//            {
//                moveTo(follow.Target.Transform.Position, follow.Range, deltaTime);
//            }
//
//            if (CurrentTask is Attack attack)
//            {
//                var target = attack.Target;
//                var targetPosition = target.Transform.Position;
//                
//                if (target != null && (target.Health < 0 || Vector3.Distance(targetPosition, Transform.Position) > MinimalShootingRange))
//                {
//                    CompleteTask();
//                    var t = Model.GameObjects.FirstOrDefault(u => u.Player != Player);
//                    Do(new Attack(t));
//                }
//                
//                var distance = Vector3.Distance(Transform.Position, targetPosition);
//                if (distance < MinimalShootingRange)
//                {
//                    foreach (var weapon in weapons)
//                    {
//                        if (distance < weapon.ShootingRange)
//                        {
//                            var orientation = targetPosition - Transform.Position;
//                            weapon.Shoot(Transform.Position, orientation, Player);
//                        }
//
//                    }
//                } else
//                {
//                    Do(new Follow(attack.Target, MinimalShootingRange));
//                }
//
//            }
//
//            foreach (var weapon in weapons)
//            {
//                weapon.Update(deltaTime);
//            }
        }
    }
}