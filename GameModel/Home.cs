using System.Linq;

namespace GameModel
{
    public class Home : Unit
    {
        private float RestTime = 1.5f;

        private float restTime = 0.0f;

        private float HealingRadius = 5.0f;
        
        public Turret Turret1 { get; set; }
        
        public Turret Turret2 { get; set; }

        public Home(Model model, Vector position, Player player) : base(model)
        {
            Radius = HealingRadius;
            Player = player;

            Position = position;

            Turret1 = new Turret(model) {Player = Player, Position = Position + new Vector(Radius, 3.0f, 0.0f)};
            
            Turret2 = new Turret(model) {Player = Player, Position = Position + new Vector(-Radius, 3.0f, 0.0f)};
            
            Turret1.AddWeapon(model.weaponTypes[0].GetInstance());
            Turret2.AddWeapon(model.weaponTypes[0].GetInstance());
            Orientation = new Vector(1.0f, 0.0f, 0.0f);
            model.AddUnit(Turret1);
            model.AddUnit(Turret2);
        }
        
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (restTime < 0.0f)
            {
                var unitsAtHome = model.Units.Where(u => u.Position.Distance(Position) < HealingRadius);

                foreach (var unit in unitsAtHome)
                {
                    unit.Heal(10.0f);
                }
                
                restTime = RestTime;
            }

            restTime -= deltaTime;
        }
    }
}