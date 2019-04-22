using System.Collections.Generic;

namespace GameModel
{
    public class WeaponType
    {
        public string Name = "Weapon";
        
        public float ShootingRange { get; set; } = 15.0f;

        public float Speed { get; set; } = 1.0f;

        public float Damage { get; set; } = 1.0f;

        public float TimeToReload = 0.5f;

        private List<Bullet> bullets = new List<Bullet>();
        public List<Bullet> Bullets => bullets;
        public WeaponType()
        {
        }

        public void Add(Bullet b)
        {
            bullets.Add(b);
        }
        
        public void Update(float deltaTime)
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

        public Weapon GetInstance()
        {
            return new Weapon(this);
        }
    }
}