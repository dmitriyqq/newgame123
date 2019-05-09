namespace GameModel
{
    public class TempUnit : Unit
    {
        public TempUnit(Model model) : base(model)
        {
            
        }
        
        public IPhysicsBody body { get; set; }

        public override void Update(float deltaTime)
        {
            if (body != null)
            {
                Position.X = body.Position.X;
                Position.Y = body.Position.Y;
                Position.Z = body.Position.Z;
            }
        }
    }
}