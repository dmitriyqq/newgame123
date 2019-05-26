using System.Numerics;

namespace GameModel
{
    public class Bullet
    {
        public Vector3 Velocity;

        public float Damage;

        public Vector3 Position;

        public Player Player;

        public float Life = 4.0f;

        public Bullet(Vector3 position, Vector3 direction, float damage, Player player, float speed)
        {
            Position = position;
            Velocity = direction * speed;
            Damage = damage;
            Player = player;
        }
    }
}