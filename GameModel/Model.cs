using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameModel.GameObjects;
using GameModel.Tasks;

namespace GameModel
{
    public class Model
    {
        public event Action<GameObject> OnAddGameObject;
        public event Action<GameObject> OnRemoveGameObject;

        public IPhysicsEngine Engine { get; set; }
        
        private readonly List<GameObject> _gameObjects = new List <GameObject>();
        private List<GameObject> InitialState { get; set; }
        private List<Player> Players { get; } = new List <Player>();
        public List<WeaponType> WeaponTypes { get; private set; } = new List<WeaponType>();
        public int Ticks { get; private set; } = -1;
        public int NumObjects => _gameObjects.Count;
        public float Time { get; private set; }
        public Logger Logger { get; private set; }

        public IEnumerable<GameObject> GameObjects => _gameObjects;

        public Model(List<GameObject> initialState)
        {
            InitialState = initialState;

            Logger = new Logger("Model");
            Logger.Info("Created model");

            Players.Add(new Player("Human", new Vector3(0.0f, 0.0f, 1.0f)));
            Players.Add(new Player("Computer", new Vector3(1.0f, 0.0f, 0.0f)));

            
            WeaponTypes.Add(new WeaponType());
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
            for (int i = _gameObjects.Count - 1; i >= 0; i--)
            {
                var unit = _gameObjects[i];

                if (unit.Health < 0.0f)
                {
                    _gameObjects[i] = _gameObjects[_gameObjects.Count - 1];
                    OnRemoveGameObject?.Invoke(unit);
                    _gameObjects.RemoveAt(_gameObjects.Count - 1);
                }
                else
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
    }
}