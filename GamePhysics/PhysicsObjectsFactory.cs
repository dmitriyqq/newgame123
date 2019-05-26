using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using GameModel;
using MathFloat;

namespace GamePhysics
{
    public class PhysicsObjectsFactory
    {
        private Simulation _simulation;
        private Logger _logger;
        private BufferPool _bufferPool;
        
        public PhysicsObjectsFactory(Simulation simulation, BufferPool bufferPool, Logger logger)
        {
            _simulation = simulation;
            _logger = logger;
            _bufferPool = bufferPool;
        }
        
        public int CreatePhysicsBody(GameObject gameObject)
        {
            if (gameObject is Map map)
            {
                return AddMap(map);
            }
            else
            {
                return AddSphere(gameObject);
            }
        }
        
        private static Vector3 GetVertexInPosition(Map map, int i, int j)
        {
            float y = map.data[i, j];
            float x = (i - map.Size / 2) * map.Resolution;
            float z = (j - map.Size / 2) * map.Resolution;

            return new Vector3(x, y, z);
        }
        private int ConstructData(Map map)
        {
            var points = new QuickList<Vector3>(map.Size * map.Size, _bufferPool);

            CreateDeformedPlane(map.Size, map.Size,
                (int vX, int vY) =>
                {
                    var p1 = GetVertexInPosition(map, vX, vY);
                    return p1;
                }, new Vector3(1, 1, 1), _bufferPool, out var planeMesh);
            
            var idx = _simulation.Statics.Add(
                new StaticDescription(new Vector3(0, 0, 0),
                BepuUtilities.Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float) Math.PI / 2.0f),
                new CollidableDescription(_simulation.Shapes.Add(planeMesh), 0.1f)));

            map.Handle = idx;
            
            points.Span.Slice(0, map.Size * map.Size);
            return idx;
        }

        public static void CreateDeformedPlane(int width, int height, Func<int, int, Vector3> deformer, Vector3 scaling, BufferPool pool, out Mesh mesh)
        {
            pool.Take<Vector3>(width * height, out var vertices);
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    vertices[width * j + i] = deformer(i, j);
                }
            }

            var quadWidth = width - 1;
            var quadHeight = height - 1;
            var triangleCount = quadWidth * quadHeight * 2;
            pool.Take<Triangle>(triangleCount, out var triangles);
            triangles = triangles.Slice(0, triangleCount);

            for (int i = 0; i < quadWidth; ++i)
            {
                for (int j = 0; j < quadHeight; ++j)
                {
                    var triangleIndex = (j * quadWidth + i) * 2;
                    ref var triangle0 = ref triangles[triangleIndex];
                    ref var v00 = ref vertices[width * j + i];
                    ref var v01 = ref vertices[width * j + i + 1];
                    ref var v10 = ref vertices[width * (j + 1) + i];
                    ref var v11 = ref vertices[width * (j + 1) + i + 1];
                    triangle0.A = v00;
                    triangle0.B = v01;
                    triangle0.C = v10;
                    ref var triangle1 = ref triangles[triangleIndex + 1];
                    triangle1.A = v01;
                    triangle1.B = v11;
                    triangle1.C = v10;
                }
            }
            pool.Return(ref vertices);
            mesh = new Mesh(triangles, scaling, pool);
        }

        public int AddMap(Map map)
        {
            _logger.Info("Adding map");
            return ConstructData(map);
        }

        public int AddSphere(GameObject gameObject)
        {
            var r = gameObject.Radius;
            var shape = new Sphere(r);
            var idx = _simulation.Shapes.Add(shape);
            shape.ComputeInertia(50.0f, out var bodyInertia);
            var rp = new RigidPose(Vector3Helper.Random() * 10.0f + new Vector3(50.0f, 50.0f, 50.0f));
            var cd = new CollidableDescription(idx, 0.0f);
            var bad = new BodyActivityDescription(0.01f);
            var bd = BodyDescription.CreateDynamic(rp, bodyInertia, cd, bad);
            var handle = _simulation.Bodies.Add(bd);
            return handle;
        }
    }
}