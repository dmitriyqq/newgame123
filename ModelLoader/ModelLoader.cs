using System.Collections.Generic;
using GameModel;
using GamePhysics;

namespace ModelLoader
{
    public class ModelLoader
    {
        private Model buildModel()
        {
            var physicsEngine = new PhysicsEngine();
            var map = new Map(100);
            var initialState = new List<GameObject> { map };
            var model = new Model(initialState);
            model.Use(physicsEngine);
            return model;
        }

        public Model CreateEmptyModelWithMap()
        {
            return buildModel();
        }

        public Model LoadModel(string path)
        {
            return buildModel();
        }
    }
}