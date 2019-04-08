using System;
using System.Numerics;

namespace GameModel
{
    public class Unit
    {
        public Vector Position;

        public Vector Orientation;

        public float Velocity;

        public float Acceleration;

        public Player Player;
        
        public float Mass;

        public float Radius = 0.7f;

        public Task currentTask;

        public Unit Target;

        public void Update(float deltaTime)
        {
            Position += Orientation * Velocity * deltaTime;
            Velocity += Acceleration;
        }

        public bool IsIntersect(Vector start, Vector direction)
        {
            var L = Position - start;
            var tc = L.dotProduct(direction);
            if (tc < 0.0)
            {
                return false;
            }

            var l = L.Length();
            
            float d = (float) Math.Sqrt((tc*tc) - (l*l));
            if ( d > Radius) return false;
	
            //solve for t1c
            float t1c = (float) Math.Sqrt( (Radius * Radius) - (d*d) );
	
            return true;
        }

        public bool SelectPosition(Vector position)
        {
            return false;
        }

        public bool SelectUnit(Unit u)
        {
            return false;
        }
    }
}