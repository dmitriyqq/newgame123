using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using BepuUtilities;
using GameModel.Tasks;
using Quaternion = BepuUtilities.Quaternion;

namespace GameModel
{
    public class GameObject
    {
        public float Health = 100.0f;
        public RigidTransform Transform;
        public int Handle;
        public Player Player;
        public float Radius = 1.1f;
        public bool Static;

        private readonly Stack<Task> tasks = new Stack<Task>();
        private readonly List<Weapon> weapons = new List<Weapon>();

        public Task CurrentTask => tasks.Count != 0 ? tasks.Peek() : null;
        public float MinimalShootingRange { get; private set; } = -1;
        public Model Model { private get; set; }

        public object Asset;

        public GameObject()
        {
            Transform = new RigidTransform {Orientation = Quaternion.Identity};
        }
        
        public virtual void Update(float deltaTime)
        {
            Transform = Model.Engine.GetTransform(Handle);
            
//            if (CurrentTask is Move move)
//            {
//                moveTo(move.Target, 1.0f, deltaTime);
//            }
//
//            if (CurrentTask is Follow follow)
//            {
//                var targetPosition = follow.Target.Transform.Position;
//                moveTo(targetPosition, follow.Range, deltaTime);
//            }
//
//            if (CurrentTask is Attack attack)
//            {
//                var targetPosition = attack.Target.Transform.Position;
//                var distance = Vector3.Distance(Transform.Position, targetPosition);
//                if (distance < MinimalShootingRange)
//                {
//                    foreach (var weapon in weapons)
//                    {
//                        if (distance < weapon.ShootingRange)
//                        {
//                            var orientation = targetPosition - Transform.Position;
//                            weapon.Shoot(Transform.Position, orientation, Player);
//                        }
//
//                    }
//                } else
//                {
//                    Do(new Follow(attack.Target, MinimalShootingRange));
//                }
//
//            }
//
//            foreach (var weapon in weapons)
//            {
//                weapon.Update(deltaTime);
//            }
        }

        public virtual void AddWeapon(Weapon w)
        {
            MinimalShootingRange = MinimalShootingRange < 0.0f ? w.ShootingRange : Math.Min(w.ShootingRange, MinimalShootingRange);
            weapons.Add(w);
        }
        
        public virtual void Do(Task task)
        {
            tasks.Push(task);
        }

        public void CompleteTask()
        {
            tasks.Pop();
        }

        protected void moveTo(Vector3 target, float range, float deltaTime)
        {
//            if (Position.Distance(target) < range)
//            {
//                CompleteTask();
//                Acceleration = 0.0f;
//            }

//            var targetOrientation = (target - Position).Normalize();
//            var diff = (targetOrientation - Orientation).Normalize();

//            if (diff.Length() > 0.1f)
//            {
//                var c = Math.Min(RotationSpeed * deltaTime, diff.Length());
//                Orientation += diff * c;
//            }
            
//            if (Velocity < MaxVelocity)
//            {
//                Acceleration = Math.Min(1.0f / diff.Length(), 0.02f);
//            }

//            if (Position.Distance(target) <= range + 2.0f * Velocity)
//            {
//                Acceleration = -0.03f;
//            }
        }

        public Vector3? IsIntersect(Vector3 start, Vector3 direction)
        {
            // sc = Position
            // p = start
            // d = direction
            // sr = radius

            var m = start - Transform.Position; 
            var b = Vector3.Dot(m, direction); 
            var c =  Vector3.Dot(m, m) - Radius * Radius; 

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
            if (c > 0.0f && b > 0.0f) return null; 
            var discr = b * b - c; 

            // A negative discriminant corresponds to ray missing sphere 
            if (discr < 0.0f) return null; 

            // Ray now found to intersect sphere, compute smallest t value of intersection
            var t = -b - (float) Math.Sqrt(discr); 

            // If t is negative, ray started inside sphere so clamp t to zero 
            if (t < 0.0f) t = 0.0f; 
            var q = start + direction * t; 

            return q;
        }

        public bool SelectPosition(Vector3 position)
        {
            return false;
        }

        public bool SelectUnit(GameObject u)
        {
            return false;
        }

        public void Heal(float healAmount)
        {
            Health += healAmount;
        }
    }
}