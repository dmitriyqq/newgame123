namespace GameModel.Tasks
{
    public class Attack : Task
    {
        public GameObject Target;

        public Attack(GameObject target)
        {
            Target = target; 
        }
    }
}