using System.Configuration;

namespace GameModel.Tasks
{
    public class Follow : Task
    {
        public GameObject Target;

        public float Range;

        public Follow(GameObject target, float range)
        {
            Target = target;
            Range = range;
        }
    }
}