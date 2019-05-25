using GameModel;
using GameRenderer;
using GameUI;
using GamePhysics;

namespace RunDesktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var modelLoader = new ModelLoader.ModelLoader();
            var model = modelLoader.CreateEmptyModelWithMap();
            var renderer = new Renderer(model);
            var ui = new UserInterface(renderer, model, model.engine);
            renderer.AddUserInterface(ui);

            var loggers = new[]
            {
                (model.engine as PhysicsEngine)?.Logger,
                model.Logger,
                renderer.Logger,
                ui.Logger
            };

            var sinks = new[]
            {
                new ConsoleLoggerSink(),
                ui.CreateLoggerSink()
            };

            foreach (var logger in loggers)
            {
                foreach (var sink in sinks)
                {
                    sink.Subscribe(logger);
                }
            }
            
            renderer.Run(60, 60);
        }
    }
}