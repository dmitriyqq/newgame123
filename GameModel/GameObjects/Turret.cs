using System;
using System.Linq;
using GameModel.Tasks;

namespace GameModel
{
    public class Turret : GameObject
    {
        public GameObject target = null;

        public float restTime;

        public float RestTime = 1.5f;

        public Turret()
        {
            Acceleration = 0.0f;
            Velocity = 0.0f;
            MaxVelocity = 0.0f;
        }

        public override void AddWeapon(Weapon w)
        {
            w.ShootingRange = 50.0f;
            base.AddWeapon(w);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Check if target not valid
            if (target != null && (target.Health < 0 || target.Position.Distance(Position) > MinimalShootingRange))
            {
                target = null;
            }
            
            // get new target
            if (target == null && restTime < 0.0f)
            {
                target = Model.GameObjects.FirstOrDefault(u => u.Player != Player && u.Position.Distance(Position) < MinimalShootingRange);
                restTime = RestTime;
            } 
            
            // Shoot
            if (target != null)
            {
                var distance = Position.Distance(target.Position);
                if (distance < MinimalShootingRange)
                {
                    foreach (var weapon in weapons)
                    {
                        if (distance < weapon.ShootingRange)
                        {
                            var orientation = target.Position - Position;
                            weapon.Shoot(Position, orientation, Player);
                        }

                    }
                } 
            }
            
            restTime -= deltaTime;

            if (target != null)
            {
//                Orientation = (target.Position - Position).Normalize();
            }
             
        }
    }
}