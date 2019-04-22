using System;
using System.Collections.Generic;

namespace GameModel
{
    public class Weapon
    {
        private WeaponType Type;
        public float ShootingRange { get; set; } = 15.0f;
        public float Speed { get; set; } = 1.0f;
        public float Damage { get; set; } = 1.0f;

        public float TimeToReload = 0.5f;

        public float Pause = 0.0f;

        public Weapon(WeaponType weapon)
        {
            Type = weapon;
        }

        public void Shoot(Vector position, Vector direction, Player Player)
        {
            if (Pause <= 0.0f)
            {
                Console.WriteLine("shoot");
                Type.Add(new Bullet(position, direction, Damage, Player, Speed));
                Pause = TimeToReload;
            }
        }

        public void Update(float deltaTime)
        {
            if (Pause >= 0.0f)
            {
                Pause -= deltaTime;
            }
        }
    }
}