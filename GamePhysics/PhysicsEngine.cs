using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using GameModel;
using GameRenderer;
using Vector = GameModel.Vector;

namespace GamePhysics
{
    public class PhysicsEngine : IPhysicsEngine
    {
        private Vector3 getVertexInPosition(int i, int j)
        {
            float z = map.data[i, j];
            float x = (i - map.Size / 2) * map.Resolution;
            float y = (j - map.Size / 2) * map.Resolution;

            return new Vector3(x, z, y);
        }

        private QuickList<Vector3> ConstructData()
        {
            var points = new QuickList<Vector3>(map.Size * map.Size * 2, bufferPool);
            
            for (int i = 0; i < map.Size; i++)
            {
                for (int j = 0; j < map.Size; j++)
                {
                    var p1 = getVertexInPosition(i, j);
                    points.AllocateUnsafely() = p1;
                }
            }

            return points;
        }

        public Logger Logger;
        
        private TypedIndex mapShapeIndex;
        private int mapBodyIndex;
        private Simulation simulation;
        private ConvexHull mapHull;
        private Map map;
        private BufferPool bufferPool;
        private HullData hullData;
        public PhysicsEngine()
        {
            Logger = new Logger("Physics");
            Logger.Info("Created physics engine");

            bufferPool = new BufferPool();
            simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(Logger), 
                new IntegratorCallbacks.PoseIntegratorCallbacks(new Vector3(0.0f, -10.0f, 0.0f), Logger));
        }
        
//        public void AddMap(Map map) {
//            try
//            {
//                Logger.Info("Adding map");
//                this.map = map;
//                mapHull = new ConvexHull();
//
//                var pointsBuffer = ConstructData().Span.Slice(0, map.Size * map.Size);            
//                ConvexHullHelper.CreateShape(pointsBuffer, bufferPool, out hullData, out Vector3 tmp, out mapHull);
//
//                mapShapeIndex = simulation.Shapes.Add(mapHull);
//                var mapBodyDescription = new StaticDescription(new Vector3(0, 0, 0), new CollidableDescription(mapShapeIndex, 0.1f));
//                mapBodyIndex = simulation.Statics.Add(mapBodyDescription);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//                throw;
//            }
//        }

        public void Update(float deltaTime)
        {
            simulation.Timestep(deltaTime);
        }

        public void AddGameObject(GameObject gameObject)
        {
            if (gameObject is TempGameObject tu)
            {
                var shape = new Sphere(2.0f);
                var idx = simulation.Shapes.Add(shape);

                var rp = new RigidPose(new Vector3(20.0f, 150.0f, 39.0f));
                shape.ComputeInertia(50.0f, out var bodyInertia);
                
                var cd = new CollidableDescription(idx, 0.0f);
                var bad = new BodyActivityDescription();
                var bd = BodyDescription.CreateDynamic(rp, bodyInertia, cd, bad);
                
                var adaptor = new BodyAdaptor(bd);
                adaptor.Handle = simulation.Bodies.Add(bd);
                tu.body = adaptor;
            }   
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (gameObject is TempGameObject tu)
            {
                simulation.Bodies.Remove((tu.body as BodyAdaptor)?.Handle ?? 0);
            }
        }

        public event Action<GameObject, GameObject> OnCollision;
    }
}