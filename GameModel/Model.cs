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
        private List<GameObject> InitialState { get; }
        private List<Player> Players { get; } = new List <Player>();
        public List<WeaponType> WeaponTypes { get; private set; } = new List<WeaponType>(); 
        public int Ticks { get; private set; }
        public int NumObjects => _gameObjects.Count;
        public float Time { get; private set; }
        public Logger Logger { get; private set; }

        public IEnumerable<GameObject> GameObjects => _gameObjects;

        public Model(List<GameObject> initialState)
        {
            this.InitialState = initialState;

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

            foreach (var go in InitialState)
            {
                AddGameObject(go);
            }
        }

        public void AddGameObject(GameObject u)
        {
            u.Model = this;
            OnAddGameObject?.Invoke(u);
            _gameObjects.Add(u);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            OnRemoveGameObject?.Invoke(gameObject);
            _gameObjects.Remove(gameObject);
        }

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

            if (_gameObjects.Count < 200)
            {
                var weaponType = WeaponTypes.First();
                
                var unit = new ArmyGameObject();
                unit.Player = Players[0];
                // unit.Position = Vector.Random() * 25.0f - new Vector(25.0f, 0.0f, 25.0f);
                // unit.Orientation = QuaternionHelper.Random();
                unit.AddWeapon(weaponType.GetInstance());
                AddGameObject(unit);
                
                
                var unit2 = new ArmyGameObject();
                unit2.Player = Players[1];
//                unit2.Position = Vector.Random() * 25.0f - new Vector(-25.0f, 0.0f, 25.0f);
//                unit2.Orientation = QuaternionHelper.Random();
                unit2.AddWeapon(weaponType.GetInstance());
                AddGameObject(unit2);

                unit.Do(new Attack(unit2));
                unit2.Do(new Attack(unit));
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