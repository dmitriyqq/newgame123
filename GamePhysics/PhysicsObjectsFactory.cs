using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using GameModel;
using GameModel.GameObjects;
using MathFloat;

namespace GamePhysics
{
    public class PhysicsObjectsFactory
    {
        private readonly Simulation _simulation;
        private readonly Logger _logger;
        private readonly BufferPool _bufferPool;
        
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
            
            return AddSphere(gameObject);
        }
        
        private static Vector3 GetVertexInPosition(Map map, int i, int j)
        {
            var y = map.Data[i, j].Height;
            var x = (i - map.Size / 2) * map.Resolution;
            var z = (j - map.Size / 2) * map.Resolution;

            return new Vector3(x, y, z);
        }
        private int ConstructMap(Map map)
        {
            var mapMesh = CreateDeformedPlane(map);
            
            var idx = _simulation.Statics.Add(
                new StaticDescription(new Vector3(0, 0, 0),
                BepuUtilities.Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float) Math.PI / 2.0f),
                new CollidableDescription(_simulation.Shapes.Add(mapMesh), 0.1f)));

            map.Handle = idx;
            return idx;
        }

        private Buffer<Triangle> _triangles;
        private void UpdateMap(Map map)
        {
            _logger.Info("Updating map");

            _bufferPool.Return(ref _triangles);
            _simulation.Statics.Remove(map.Handle);

            var mapMesh = CreateDeformedPlane(map);
            var idx = _simulation.Statics.Add(
                new StaticDescription(new Vector3(0, 0, 0),
                    BepuUtilities.Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float) Math.PI / 2.0f),
                    new CollidableDescription(_simulation.Shapes.Add(mapMesh), 0.1f)));

            map.Handle = idx;
        }


        private Mesh CreateDeformedPlane(Map map)
        {
            var width = map.Size;
            var height = map.Size;
            var quadWidth = width - 1;
            var quadHeight = height - 1;
            var triangleCount = quadWidth * quadHeight * 2;
            
            _bufferPool.Take<Vector3>(width * height, out var vertices);
            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    vertices[width * j + i] = GetVertexInPosition(map, i, j);
                }
            }

            _bufferPool.Take<Triangle>(triangleCount, out _triangles);
            _triangles = _triangles.Slice(0, triangleCount);
            for (var i = 0; i < quadWidth; ++i)
            {
                for (var j = 0; j < quadHeight; ++j)
                {
                    var triangleIndex = (j * quadWidth + i) * 2;
                    ref var triangle0 = ref _triangles[triangleIndex];
                    ref var v00 = ref vertices[width * j + i];
                    ref var v01 = ref vertices[width * j + i + 1];
                    ref var v10 = ref vertices[width * (j + 1) + i];
                    ref var v11 = ref vertices[width * (j + 1) + i + 1];
                    triangle0.A = v00;
                    triangle0.B = v01;
                    triangle0.C = v10;
                    ref var triangle1 = ref _triangles[triangleIndex + 1];
                    triangle1.A = v01;
                    triangle1.B = v11;
                    triangle1.C = v10;
                }
            }
            
            var scaling = new Vector3(1.0f, 1.0f, 1.0f);
            _bufferPool.Return(ref vertices);
            return new Mesh(_triangles, scaling, _bufferPool);
        }

        private int AddMap(Map map)
        {
            _logger.Info("Adding map");
            map.OnBatchUpdate += () => { UpdateMap(map); };
            return ConstructMap(map);
        }

        private int AddSphere(GameObject gameObject)
        {
//            var r = gameObject.Radius;
            var shape = new Sphere(1.5f);
            var idx = _simulation.Shapes.Add(shape);
            shape.ComputeInertia(50.0f, out var bodyInertia);
            var rigidPose = new RigidPose(gameObject.Transform.Position, gameObject.Transform.Orientation);
            var collidableDescription = new CollidableDescription(idx, 0.0f);
            var bodyActivityDescription = new BodyActivityDescription(0.01f);
            var bodyDescription = BodyDescription.CreateDynamic(rigidPose, bodyInertia, collidableDescription, bodyActivityDescription);
            var handle = _simulation.Bodies.Add(bodyDescription);
            return handle;
        }
    }
}