using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using GameModel;
using GameRenderer;

namespace GamePhysics
{
    public class PhysicsEngine : IPhysicsEngine
    {
        public Logger Logger;
        private int mapBodyIndex;
        private Simulation simulation;
        private BufferPool bufferPool;
        private HullData hullData;
        private PhysicsObjectsFactory factory; 
        public PhysicsEngine()
        {
            Logger = new Logger("Physics");
            Logger.Info("Created physics engine");

            bufferPool = new BufferPool();
            simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(Logger), 
                new IntegratorCallbacks.PoseIntegratorCallbacks(new Vector3(0.0f, -10.0f, 0.0f), Logger));
            factory = new PhysicsObjectsFactory(simulation, bufferPool, Logger);
        }

        public void Update(float deltaTime)
        {
            simulation.Timestep(deltaTime);
        }

        public void AddGameObject(GameObject gameObject)
        {
            var handle = factory.CreatePhysicsBody(gameObject);
            gameObject.Handle = handle;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            simulation.Bodies.Remove(gameObject.Handle);
        }

        public RigidTransform GetTransform(int handle)
        {
            simulation.Bodies.GetDescription(handle, out var description);
            return new RigidTransform(description.Pose.Position, description.Pose.Orientation);
        }

        public event Action<GameObject, GameObject> OnCollision;
    }
}