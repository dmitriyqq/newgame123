using System.Collections.Generic;
using GameModel;
using GameRenderer;
using GameUI;
using ModelLoader;

namespace RunDesktop
{
    public static class Program
    {
        private const string AssetFile = "./assets/Assets.xml";
        public static void Main(string[] args)
        {
            var modelLoader = new ModelLoader.ModelLoader();

            // Get model with initial state
            var model = modelLoader.CreateEmptyModelWithMap();

            var assetStoreLogger = new Logger("Assets");
            var assetStore = new AssetStore(AssetFile, assetStoreLogger);
            
            // Renderer contains game loop
            var renderer = new Renderer(model, assetStore.AssetsFile);

            var loggers = new List<Logger>
            {
                (model.Engine as GamePhysics.PhysicsEngine)?.Logger,
                model.Logger,
                renderer.Logger,
                assetStoreLogger
            };

            var sinks = new List<ILoggerSink>
            {
                new ConsoleLoggerSink(),
                new FileLoggerSink("logs/log.txt") 
            };
            
            var loggerManager = new LoggerManager(sinks, loggers);
            
            var ui = new UserInterface(renderer, model, loggerManager);
            renderer.AddUserInterface(ui);
            ui.AddRendererMenu(renderer);
            ui.Layout.AddConstructor(assetStore, renderer.Camera, renderer);
            ui.Layout.AddEventView();
            ui.Layout.AddModelControls(renderer, renderer.Camera);
            ui.Layout.AddMapTools(ui, renderer.Camera);

            loggerManager.AddLogger(ui.Logger);
            loggerManager.AddSink(ui.CreateLoggerSink());
            
            renderer.Run(60, 60);
        }
    }
}