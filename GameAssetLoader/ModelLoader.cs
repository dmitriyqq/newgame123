using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GameModel;
using GameModel.GameObjects;

namespace ModelLoader
{
    public class ModelLoader
    {
        private Model BuildModel()
        {
            var physicsEngine = new GamePhysics.PhysicsEngine();
            var map = new Map{Size = 100};
            map.CreateMap();

            var initialState = new List<GameObject> { map };
            var model = new Model() { InitialState = initialState};

            model.Use(physicsEngine);
            return model;
        }

        public Model CreateEmptyModelWithMap()
        {
            return BuildModel();
        }

        public Model LoadModel(string path)
        {
            var serializer = new XmlSerializer(typeof(GameSave));
            using (var stream = new StreamReader(path))
            {
                var gameSave = serializer.Deserialize(stream);
                if (gameSave is GameSave save)
                {
                    var model = new Model(save);
                    var physicsEngine = new GamePhysics.PhysicsEngine();
                    model.Use(physicsEngine);
                    return model;
                }
                
                throw new XmlException("Couldn't load game save");
            }
        }

        public void SaveModel(Model model)
        {
            var serializer = new XmlSerializer(typeof(GameSave));

            var gameSave = model.CreateSave();
            
            using ( var stream = new StreamWriter("./saves/save.xml"))
            {
                serializer.Serialize(stream, gameSave);
            }
        }
    }
}