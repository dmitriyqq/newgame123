using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Tasks;

namespace GameModel
{
    public class Model
    {
        public event Action<Unit> OnAddUnit;
        public event Action<Unit> OnRemoveUnit;

        private IPhysicsEngine engine;
        
        private List<Unit> units { get; } = new List <Unit>();
        private List<Player> players { get; } = new List <Player>();
        public List<WeaponType> weaponTypes { get; private set; } = new List<WeaponType>(); 
        public int Ticks { get; private set; }

        public Logger Logger { get; private set; }

        private Home home1;
        private Home home2;

        public Map Map;

        public Model(IPhysicsEngine engine)
        {
            Logger = new Logger("Model");
            Logger.Info("Created model");

            this.engine = engine;
            
            Map = new Map(200);

            players.Add(new Player("Human", new Vector(0.0f, 0.0f, 1.0f)));
            players.Add(new Player("Computer", new Vector(1.0f, 0.0f, 0.0f)));

            engine.AddMap(Map);

            OnAddUnit += this.engine.AddUnit;
            OnRemoveUnit += this.engine.RemoveUnit;
            
            weaponTypes.Add(new WeaponType());
        }

        public void Start()
        {
            
            CreateHomes();
        }

        private void CreateHomes()
        {
            home1 = new Home(this, new Vector(25.0f, 0.0f, 0.0f), players[0]);
            AddUnit(home1);

            home2 = new Home(this, new Vector(-25.0f, 0.0f, 0.0f), players[0]);
            AddUnit(home2);


            for (int i = 0; i < 5; i++)
            {
                AddUnit(new TempUnit(this){Player = players[0]});
                // AddUnit(new Buggy(this){Player = players[1]});
                // AddUnit(new Tank(this){Player = players[0]});
                // AddUnit(new Tank(this){Player = players[1]});
            }
        }

        private void RemoveUnit(Unit unit)
        {
            OnRemoveUnit(unit);
            units.Remove(unit);
        }
        
        public void Update(float deltaTime)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                var unit = units[i];

                if (unit.Health < 0.0f)
                {
                    units[i] = units[units.Count - 1];
                    OnRemoveUnit?.Invoke(unit);
                    units.RemoveAt(units.Count - 1);
                }
                else
                {
                    unit.Update(deltaTime);
                }

            }

            if (Ticks % 100 == 5 && units.Count < 20)
            {
                var weaponType = weaponTypes.First();
                
                var unit = new ArmyUnit(this);
                unit.Player = players[0];
                unit.Position = Vector.Random() * 25.0f - new Vector(25.0f, 0.0f, 25.0f);
                unit.Orientation = Vector.Random();
                unit.AddWeapon(weaponType.GetInstance());
                AddUnit(unit);
                
                
                var unit2 = new ArmyUnit(this);
                unit2.Player = players[1];
                unit2.Position = Vector.Random() * 25.0f - new Vector(-25.0f, 0.0f, 25.0f);
                unit2.Orientation = Vector.Random();
                unit2.AddWeapon(weaponType.GetInstance());
                AddUnit(unit2);

                unit.Do(new Attack(unit2));
                unit2.Do(new Attack(unit));
            }
            
            doCollision();

            foreach (var weapon in weaponTypes)
            {
                weapon.Update(deltaTime);
            }

            Logger.Info("Engine updating");
            engine.Update(deltaTime);
            Ticks++;
        }

        private void doCollision()
        {
//            for (int i = 0; i < units.Count; i++)
//            {
//                var a = units[i];
//                for (int j = i + 1; j < units.Count; j++)
//                {
//                    
//                    var b = units[j];
//
//                    var diff = b.Position - a.Position;
//                    var l = diff.Length();
//                    var d = diff.Normalize();
//                    if (l < a.Radius + b.Radius)
//                    {
//                        a.Position += d * (-l / 4);
//                        b.Position += d * ( l / 4);
//                    }
//                }
//
//                foreach (var weapon in weaponTypes)
//                {
//                    var bullets = weapon.Bullets;
//                    for (var j = bullets.Count - 1; j >= 0; j--)
//                    {
//                        if (bullets[j].Player == a.Player)
//                        {
//                            continue;
//                        }
//
//                        var diff = bullets[j].Position - a.Position;
//                        if (diff.Length() < 0.1f)
//                        {
//                            a.Health -= 1.0f;
//                            bullets[j] = bullets[bullets.Count - 1];
//                            bullets.RemoveAt(bullets.Count - 1);
//                        }
//                    }
//                }
//            }
        }
        public void AddUnit(Unit u)
        {
            OnAddUnit(u);
            units.Add(u);
        }

        public IEnumerable<Unit> Units => units;
    }
}