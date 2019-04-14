using System.Numerics;

namespace GameModel
{
    public class Bullet
    {
        public Vector Velocity;

        public float Damage;

        public Vector Position;

        public Vector Color;

        public float Life = 4.0f;

        public Bullet(Vector position, Vector direction, float damage, Vector color, float speed)
        {
            Position = position;
            Velocity = direction * speed;
            Damage = damage;
            Color = color;
        }
    }
}