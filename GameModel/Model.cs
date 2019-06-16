using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using GameModel.GameObjects;

namespace GameModel
{
    public class Model
    {
        private IEnumerable<Type> _gameObjectTypes;

        private readonly List<GameObject> _gameObjects = new List <GameObject>();
        private List<Player> Players { get; } = new List <Player>();
        public IPhysicsEngine Engine { get; set; }
        public List<GameObject> InitialState { get; set; } = new List<GameObject>();
        public List<WeaponType> WeaponTypes { get; private set; } = new List<WeaponType>();
        public int Ticks { get; private set; }
        public float Time { get; private set; }
        public Logger Logger { get; private set; }
        public IEnumerable<GameObject> GameObjects => _gameObjects;
        public int NumObjects => _gameObjects.Count;
        public IEnumerable<Type> GameObjectTypes => _gameObjectTypes;
        
        public event Action<GameObject> OnAddGameObject;
        public event Action<GameObject> OnRemoveGameObject;

        public Model()
        {
            Logger = new Logger("Model");
            Logger.Info("Creating model from scratch");

            Players.Add(new Player{Name = "Human", Color = new Vector3(0.0f, 0.0f, 1.0f)});
            Players.Add(new Player{Name = "Computer", Color = new Vector3(1.0f, 0.0f, 0.0f)});

            WeaponTypes.Add(new WeaponType());

            GetGameObjectTypes();
        }
        
        public Model(GameSave save)
        {
            Logger = new Logger("Model");
            Logger.Info("Creating model from save");
            
            Players = save.Players;

            foreach (var gameObject in save.GameObjects)
            {
                InitialState.Add(gameObject);
            }

            Time = save.Time;
            Ticks = save.Ticks;
        }

        public GameSave CreateSave()
        {
            var save = new GameSave
            {
                Players = Players,
                GameObjects = _gameObjects,
                Ticks = Ticks,
                Time = Time
            };

            return save;
        }

        public void Use(IPhysicsEngine engine)
        {
            Engine = engine;

            OnAddGameObject += engine.AddGameObject;
            OnRemoveGameObject += engine.RemoveGameObject;
        }
        public void Start()
        {
            if (InitialState == null)
            {
                return;
            }

            Ticks = 0;
            foreach (var go in InitialState)
            {
                AddGameObject(go);
            }

            InitialState = null;
        }

        public void AddGameObject(GameObject u)
        {
            Logger.Info("AddGameObject");
            if (Ticks == -1)
            {
                Logger.Info("Add to initial state");
                InitialState.Add(u);
                return;
            }
            
            Logger.Info("Add at run");
            u.Model = this;
            OnAddGameObject?.Invoke(u);
            _gameObjects.Add(u);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            OnRemoveGameObject?.Invoke(gameObject);
            _gameObjects.Remove(gameObject);
        }

        public GameObject GameObjectByHandle(int handle) =>
            _gameObjects.FirstOrDefault(go => go.Static == false && go.Handle == handle);
        
        public void Update(float deltaTime)
        {
            Time += deltaTime;
            for (var i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var unit = _gameObjects[i];
//
//                if (unit.Health < 0.0f)
//                {
//                    _gameObjects[i] = _gameObjects[_gameObjects.Count - 1];
//                    OnRemoveGameObject?.Invoke(unit);
//                    _gameObjects.RemoveAt(_gameObjects.Count - 1);
//                }
//                else
                {
                    unit.Update(deltaTime);
                }

            }

            foreach (var weapon in WeaponTypes)
            {
                weapon.Update(deltaTime);
            }

            Engine.Update(deltaTime);
            Ticks++;
        }

        private void GetGameObjectTypes()
        {
            _gameObjectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Namespace != null && t.Namespace.Contains("GameObjects"));
        }
    }
}