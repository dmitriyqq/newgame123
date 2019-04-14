namespace GameModel.Tasks
{
    public class Attack : Task
    {
        public Unit Target;

        public Attack(Unit target)
        {
            Target = target; 
        }
    }
}