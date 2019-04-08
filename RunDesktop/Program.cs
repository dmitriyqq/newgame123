using GameModel;
using GameRenderer;

namespace RunDesktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var model = new Model();
            var renderer = new Renderer(model);
            
            renderer.Run(60, 60);
        }
    }
}