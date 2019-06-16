using System;
using System.Numerics;
using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using GameModel;
using GameModel.GameObjects;
using PhysicsEngine;

namespace GamePhysics
{
    public class PhysicsEngine : IPhysicsEngine
    {
        public readonly Logger Logger;
        private readonly Simulation _simulation;
        private readonly PhysicsObjectsFactory _factory;
        private readonly PhysicsEngineVisualizer _visualizer;
        private BufferPool _bufferPool;
        public PhysicsEngine()
        {
            Logger = new Logger("Physics");
            Logger.Info("Created physics engine");

            _bufferPool = new BufferPool();
            _simulation = Simulation.Create(_bufferPool, new NarrowPhaseCallbacks(Logger), 
                new IntegratorCallbacks.PoseIntegratorCallbacks(new Vector3(0.0f, -10.0f, 0.0f), Logger));
            _factory = new PhysicsObjectsFactory(_simulation, _bufferPool, Logger);
        }

        public void Update(float deltaTime)
        {
            _simulation.Timestep(deltaTime);
        }

        public void AddGameObject(GameObject gameObject)
        {
            var handle = _factory.CreatePhysicsBody(gameObject);
            gameObject.Handle = handle;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            _simulation.Bodies.Remove(gameObject.Handle);
        }

        public RigidTransform GetTransform(int handle)
        {
            _simulation.Bodies.GetDescription(handle, out var description);
            return new RigidTransform(description.Pose.Position, description.Pose.Orientation);
        }

        public Vector3? IntersectMap(Map map, Vector3 start, Vector3 dir, out Vector3 normal)
        {
            var handler = new MapHitHandler(map);
            
            _simulation.RayCast(start, dir, 1000.0f, ref handler);
            normal = handler.Normal;
            return handler.Collision;
        }
        
        public int? IntersectGameObjects(Vector3 start, Vector3 dir)
        {
            var handler = new HitHandler();
            _simulation.RayCast(start, dir, 1000.0f, ref handler);
            return handler.CollidableReference?.Handle;
        }
        
        public event Action<GameObject, GameObject> OnCollision;
    }
}