using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GameModel;
using GlmNet;

namespace GameRenderer
{
    public class BulletParticleEngine : ParticleEngine, IDrawable
    {
        public List<Bullet> Bullets;

        public BulletParticleEngine(List<Bullet> bullets)
        {
            Bullets = bullets;
        }

        void IDrawable.Update(float deltaTime)
        {
            var list = Bullets;

            var positions = new float[4 * list.Count];
            var colors = new float[4 * list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var index = i * 4;
                positions[index + 0] = list[i].Position.X;
                positions[index + 1] = list[i].Position.Y;
                positions[index + 2] = list[i].Position.Z;
                positions[index + 3] = 1.0f;

                var color = list[i].Player.Color;
                colors[index + 0] = color.X;
                colors[index + 1] = color.Y;
                colors[index + 2] = color.Z;
                colors[index + 3] = 1.0f;
            }

            base.Update(positions, colors, list.Count);
        }

      
    }
}