using System;
using System.Collections.Generic;

namespace GameModel
{
    public class Weapon
    {
        public float ShootingRange { get; set; } = 15.0f;

        public static List<Bullet> bullets = new List<Bullet>();

        public float Speed { get; set; } = 1.0f;

        public float Damage { get; set; } = 1.0f;

        public float TimeToReload = 0.5f;

        public float Pause = 0.0f;
        
        public void Shoot(Vector position, Vector direction, Vector color)
        {
            if (Pause <= 0.0f)
            {
                Console.WriteLine("shoot");
                bullets.Add(new Bullet(position, direction, Damage, color, Speed));
                Pause = TimeToReload;
            }
        }

        public void UpdateInstance(float deltaTime)
        {
            if (Pause >= 0.0f)
            {
                Pause -= deltaTime;
            }
        }

        public static void Update(float deltaTime)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];

                bullet.Life -= deltaTime;

                if (bullet.Life < 0.0f)
                {
                    bullets[i] = bullets[bullets.Count - 1];
                    bullets.RemoveAt(bullets.Count - 1);
                }
                else
                {
                    bullet.Position += bullet.Velocity * deltaTime;
                }
            }
        }
    }
}