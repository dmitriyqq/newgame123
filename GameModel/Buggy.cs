using GameModel.Tasks;

namespace GameModel
{
    public class Buggy : Vehicle
    {
        public Buggy(Model model) : base(model)
        {
            Radius = 2.0f;
        }
    }
}