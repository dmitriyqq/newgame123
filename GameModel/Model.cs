using System;
using System.Collections.Generic;

namespace GameModel
{
    public class Model
    {
        public event Action<Unit> OnAddUnit;
        public List<Unit> units { get; } = new List <Unit>();
        
        public List<Player> players { get; } = new List <Player>();
        public int Ticks { get; private set; }

        public Model()
        {
            players.Add(new Player("Human", new Vector(0.0f, 0.0f, 1.0f)));
            players.Add(new Player("Computer", new Vector(1.0f, 0.0f, 0.0f)));
            
        }
        
        public void Update(float deltaTime)
        {
            foreach (var unit in units)
            {
                unit.Update(deltaTime);
            }

            if (Ticks % 100 == 5)
            {
                var unit = new Unit();
                AddUnit(unit);
                unit.Player = players[0];
                unit.Position = Vector.Random() * 25.0f - new Vector(25.0f, 0.0f, 25.0f);
                unit.Orientation = Vector.Random();
                unit.Velocity = 0.5f;
                
                unit = new Unit();
                AddUnit(unit);
                unit.Player = players[0];
                unit.Position = Vector.Random() * 25.0f - new Vector(25.0f);
                unit.Orientation = Vector.Random();
                unit.Velocity = 0.5f;
            }
            
            Ticks++;
        }

        public void AddUnit(Unit u)
        {
            OnAddUnit(u);
            units.Add(u);
        }
    }
}