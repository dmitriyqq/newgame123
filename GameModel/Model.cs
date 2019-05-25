using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Tasks;

namespace GameModel
{
    public class Model
    {
        public event Action<GameObject> OnAddGameObject;
        public event Action<GameObject> OnRemoveGameObject;

        public IPhysicsEngine engine { get; set; }
        
        private List<GameObject> gameObjects { get; } = new List <GameObject>();
        private List<GameObject> initialState { get; } = new List <GameObject>();
        private List<Player> players { get; } = new List <Player>();
        public List<WeaponType> weaponTypes { get; private set; } = new List<WeaponType>(); 
        public int Ticks { get; private set; }
        public Logger Logger { get; private set; }

        public IEnumerable<GameObject> GameObjects => gameObjects;

        public Model(List<GameObject> initialState)
        {
            this.initialState = initialState;

            Logger = new Logger("Model");
            Logger.Info("Created model");

            players.Add(new Player("Human", new Vector(0.0f, 0.0f, 1.0f)));
            players.Add(new Player("Computer", new Vector(1.0f, 0.0f, 0.0f)));

            
            weaponTypes.Add(new WeaponType());
        }

        public void Use(IPhysicsEngine engine)
        {
            this.engine = engine;

            OnAddGameObject += engine.AddGameObject;
            OnRemoveGameObject += engine.RemoveGameObject;
        }

        public void Start()
        {
            if (initialState == null)
            {
                return;
            }

            foreach (var go in initialState)
            {
                AddGameObject(go);
            }
        }

        public void AddGameObject(GameObject u)
        {
            u.Model = this;
            OnAddGameObject?.Invoke(u);
            gameObjects.Add(u);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            OnRemoveGameObject?.Invoke(gameObject);
            gameObjects.Remove(gameObject);
        }

        public void Update(float deltaTime)
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                var unit = gameObjects[i];

                if (unit.Health < 0.0f)
                {
                    gameObjects[i] = gameObjects[gameObjects.Count - 1];
                    OnRemoveGameObject?.Invoke(unit);
                    gameObjects.RemoveAt(gameObjects.Count - 1);
                }
                else
                {
                    unit.Update(deltaTime);
                }

            }

            if (Ticks % 100 == 5 && gameObjects.Count < 20)
            {
                var weaponType = weaponTypes.First();
                
                var unit = new ArmyGameObject();
                unit.Player = players[0];
                unit.Position = Vector.Random() * 25.0f - new Vector(25.0f, 0.0f, 25.0f);
                unit.Orientation = Vector.Random();
                unit.AddWeapon(weaponType.GetInstance());
                AddGameObject(unit);
                
                
                var unit2 = new ArmyGameObject();
                unit2.Player = players[1];
                unit2.Position = Vector.Random() * 25.0f - new Vector(-25.0f, 0.0f, 25.0f);
                unit2.Orientation = Vector.Random();
                unit2.AddWeapon(weaponType.GetInstance());
                AddGameObject(unit2);

                unit.Do(new Attack(unit2));
                unit2.Do(new Attack(unit));
            }
            

            foreach (var weapon in weaponTypes)
            {
                weapon.Update(deltaTime);
            }

            Logger.Info("Engine updating");
            engine.Update(deltaTime);
            Ticks++;
        }
    }
}