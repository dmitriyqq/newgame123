using System.Configuration;

namespace GameModel.Tasks
{
    public class Follow : Task
    {
        public Unit Target;

        public float Range;

        public Follow(Unit target, float range)
        {
            Target = target;
            Range = range;
        }
    }
}