using GameModel;
using GameRenderer;
using GameUI;
using GamePhysics;
using ModelLoader;

namespace RunDesktop
{
    internal static class Program
    {
        private const string AssetFile = "./assets/Assets.xml";
        public static void Main(string[] args)
        {
            var modelLoader = new ModelLoader.ModelLoader();

            // Get model with initial state
            var (model, map) = modelLoader.CreateEmptyModelWithMap();

            var assetStoreLogger = new Logger("Assets");
            var assetStore = new AssetStore(AssetFile, assetStoreLogger);
            
            // Renderer contains game loop
            var renderer = new Renderer(model, assetStore.Assets);

            var ui = new UserInterface(renderer, model);
            renderer.AddUserInterface(ui);
            ui.AddRendererMenu(renderer);
            ui.Layout.AddConstructor(assetStore, map, renderer.Camera, renderer);
            ui.Layout.AddEventView();
            ui.Layout.AddModelControls(renderer, model, renderer.Camera);
            ui.Layout.AddMapTools(map, ui, model, renderer.Camera);

            var loggers = new[]
            {
                (model.Engine as PhysicsEngine)?.Logger,
                model.Logger,
                renderer.Logger,
                ui.Logger,
                assetStoreLogger
            };

            var sinks = new[]
            {
                new ConsoleLoggerSink(),
                ui.CreateLoggerSink(),
                new FileLoggerSink("logs/log.txt") 
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