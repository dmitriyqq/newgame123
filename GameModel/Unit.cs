using System;
using System.Collections.Generic;
using System.Numerics;
using GameModel.Tasks;

namespace GameModel
{
    public class Unit
    {
        public Vector Position;

        public Vector Orientation = Vector.Random().Normalize();

        public float Velocity;

        public float Acceleration = 0.0f;

        public float MaxVelocity = 10.0f;

        public Player Player;
        
        public float Mass;

        public float Radius = 1.1f;

        public float RotationSpeed = 0.4f;

        private Stack<Task> tasks = new Stack<Task>();

        public Weapon Weapon { get; set; } = new Weapon();
        
        public Task CurrentTask => tasks.Count != 0 ? tasks.Peek() : null;

        public void Update(float deltaTime)
        {
            Position += Orientation * Velocity * deltaTime;
            Velocity += Acceleration;
            Weapon.UpdateInstance(deltaTime);
            
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
                if (Position.Distance(attack.Target.Position) < Weapon.ShootingRange)
                {
                    var orientation = attack.Target.Position - Position;
                    Weapon.Shoot(Position, orientation, Player.Color);
                }
                else
                {
                    Do(new Follow(attack.Target, Weapon.ShootingRange));
                }
            }
            
        }

        public void Do(Task task)
        {
            tasks.Push(task);
        }

        public void CompleteTask()
        {
            tasks.Pop();
        }

        private void moveTo(Vector target, float range, float deltaTime)
        {
            if (Position.Distance(target) < range)
            {
                CompleteTask();
                Acceleration = 0.0f;
            }

            var targetOrientation = (target - Position).Normalize();
            var diff = (targetOrientation - Orientation).Normalize();

            if (diff.Length() > 0.1f)
            {
                var c = Math.Min(RotationSpeed * deltaTime, diff.Length());
                Orientation += diff * c;
            }
            
            if (Velocity < MaxVelocity)
            {
                Acceleration = Math.Min(1.0f / diff.Length(), 0.01f);
            }

            if (Position.Distance(target) <= range + 2.0f * Velocity)
            {
                Acceleration = -0.01f;
            }
        }

        public Vector IsIntersect(Vector start, Vector direction)
        {
            // sc = Position
            // p = start
            // d = direction
            // sr = radius

            var m = start - Position; 
            var b = m.dotProduct(direction); 
            var c = m.dotProduct(m) - Radius * Radius; 

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
            if (c > 0.0f && b > 0.0f) return null; 
            var discr = b*b - c; 

            // A negative discriminant corresponds to ray missing sphere 
            if (discr < 0.0f) return null; 

            // Ray now found to intersect sphere, compute smallest t value of intersection
            var t = -b - (float) Math.Sqrt(discr); 

            // If t is negative, ray started inside sphere so clamp t to zero 
            if (t < 0.0f) t = 0.0f; 
            var q = start + direction * t; 

            return q;
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