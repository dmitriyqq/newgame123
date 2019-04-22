using System;
using System.Linq;
using GameModel.Tasks;

namespace GameModel
{
    public class Turret : Unit
    {
        public Unit target = null;

        public float restTime;

        public float RestTime = 1.5f;

        public Turret(Model model) : base(model)
        {
            
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
                target = model.Units.FirstOrDefault(u => u.Player != Player && u.Position.Distance(Position) < MinimalShootingRange);
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
                            Console.WriteLine("Shoot!");
                            weapon.Shoot(Position, orientation, Player);
                        }

                    }
                } 
            }
            
            restTime -= deltaTime;

            if (target != null)
            {
                Orientation = (target.Position - Position).Normalize();
            }
             
        }
    }
}