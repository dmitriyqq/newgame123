using GameModel;
using GameRenderer;
using GameUI;

namespace RunDesktop
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var physicsEngine = new PhysicsEngine.PhysicsEngine();
            var model = new Model(physicsEngine);
            var renderer = new Renderer(model);
            var ui = new UserInterface(renderer, model, physicsEngine);

            physicsEngine.AddMap(model.Map);
            
            renderer.AddUserInterface(ui);
            
            var loggers = new[]
            {
                physicsEngine.Logger,
                model.Logger,
                renderer.Logger,
                ui.Logger
            };

            var sinks = new[]
            {
                new ConsoleLoggerSink(),
                ui.CreateLoggerSink(),
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