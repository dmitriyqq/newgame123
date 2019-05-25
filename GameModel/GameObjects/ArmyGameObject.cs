using System;
using System.Linq;

namespace GameModel.Tasks
{
    public class ArmyGameObject : GameObject
    {

        public override void Update(float deltaTime)
        {
            if (CurrentTask is Move move)
            {
                moveTo(move.Target, 1.0f, deltaTime);
            }

            if (CurrentTask is Follow follow)
            {
                moveTo(follow.Target.Position, follow.Range, deltaTime);
            }

            if (CurrentTask is Attack attack)
            {
                var target = attack.Target;
                
                if (target != null && (target.Health < 0 || target.Position.Distance(Position) > MinimalShootingRange))
                {
                    CompleteTask();
                    var t = Model.GameObjects.FirstOrDefault(u => u.Player != Player);
                    Do(new Attack(t));
                }
                
                var distance = Position.Distance(attack.Target.Position);
                if (distance < MinimalShootingRange)
                {
                    foreach (var weapon in weapons)
                    {
                        if (distance < weapon.ShootingRange)
                        {
                            var orientation = attack.Target.Position - Position;
                            weapon.Shoot(Position, orientation, Player);
                        }

                    }
                } else
                {
                    Do(new Follow(attack.Target, MinimalShootingRange));
                }

            }

            foreach (var weapon in weapons)
            {
                weapon.Update(deltaTime);
            }
        }
    }
}