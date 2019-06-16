using System;
using System.Collections;
using System.Linq;
using Gwen.Control;
using OpenTK;
using OpenTK.Input;
using Font = Gwen.Font;
using System.Reflection;
using GameModel;
using GameModel.GameObjects;
using GameRenderer;
using GameRenderer.Metadata.Assets;
using GameUI.AssetViewers;
using Key = OpenTK.Input.Key;


namespace GameUI
{
    public class UserInterface : IUserInterface
    {
        private readonly LoggerManager _loggerManager;
        private readonly GameWindow _gameWindow;
        private Gwen.Renderer.OpenTK _renderer;
        private Gwen.Input.OpenTK _input;
        private Menu _menu;

        public InterfaceLayout Layout { get; private set; }
        public Logger Logger { get; }
        public Model Model { get; set; }
        public Canvas Canvas { get; private set; }
        public Toggle BlockGameInput { get; private set; }
        public Renderer Renderer { get; private set; }
        public event Action<Model> ModelReload;

        public UserInterface(Renderer renderer, Model model, LoggerManager loggerManager)
        {
            _loggerManager = loggerManager;
            _gameWindow = renderer;

            BlockGameInput = new Toggle(); 
            Renderer = renderer;
            Model = model;
            Logger = new Logger("UI");
            Logger.Info("Created ui");

            try
            {
                CreateCanvas(renderer);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            
            Renderer.KeyUp += KeyUp;
            Renderer.KeyDown += KeyDown;
            Renderer.MouseUp += MouseUp;
            Renderer.MouseDown += MouseDown;
            Renderer.MouseMove += MouseMove;
            Renderer.MouseWheel += MouseWheel;
        }

        private void CreateCanvas(GameWindow window)
        {
            _renderer = new Gwen.Renderer.OpenTK();
            var skin = new Gwen.Skin.TexturedBase(_renderer, "DefaultSkin.png"){
                DefaultFont = new Font(_renderer, "Hack", 9)
            };

            Canvas = new Canvas(skin) {ShouldDrawBackground = false};
            Canvas.SetSize (window.Width, window.Height);

            _input = new Gwen.Input.OpenTK(window);
            _input.Initialize(Canvas);
        }
 
        public void Draw()
        {
            Canvas.RenderCanvas();
        }

        public void Update(float deltaTime)
        {
            _renderer.Update(deltaTime);
        }

        public void AddRendererMenu(IGameLoop renderer)
        {
            _menu = new MenuStrip(Canvas);

            var main = _menu.AddItem("Main");
            main.Menu.AddItem("Save model").SetAction((control, eventArgs) => SaveModel());
            main.Menu.AddItem("Load model").SetAction((control, eventArgs) => LoadModel());

            var debugMenu = _menu.AddItem("Debug");
            debugMenu.Menu.AddItem("Model").SetAction((control, eventArgs) => CreateDebugWindow(Model));
            debugMenu.Menu.AddItem("Renderer").SetAction((control, eventArgs) => CreateDebugWindow(renderer));
            debugMenu.Menu.AddItem("Interface").SetAction((control, eventArgs) => CreateDebugWindow(this));
            debugMenu.Menu.AddItem("Settings").SetAction((control, eventArgs) => {CreateDebugWindow(new {settings = "settings",x = 200, y = 300});});

            var tools = _menu.AddItem("Tools");
            tools.Menu.AddItem("Map Generator").SetAction((control, eventArgs) => { CreateMapGeneratorWindow();});

            Layout = new InterfaceLayout(Canvas, Logger, this);
        }

        private void SaveModel()
        {
            var modelLoader = new ModelLoader.ModelLoader();
            try
            {
                Logger.Info("Saving model");
                modelLoader.SaveModel(Model);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void LoadModel()
        {
            var modelLoader = new ModelLoader.ModelLoader();
            try
            {
                Logger.Info("Loading model");
                var model = modelLoader.LoadModel("./saves/save.xml");
                
                _loggerManager.AddLogger(model.Logger);

                Model = model;
                Renderer.Model = model;
                ModelReload?.Invoke(model);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void CreateMapGeneratorWindow()
        {
            // Search for map game object
            var mapGameObject = Model.GameObjects.FirstOrDefault(go => go is Map);
            if (mapGameObject is Map map)
            {
                var window = new MapGenerator(Canvas, map , Logger);
            }
        }
        
        public void SetCenter(Base window)
        {
            var bounds = window.Bounds;
            var halfSizeX = (bounds.Right - bounds.Left) / 2.0f;
            var halfSizeY = (bounds.Top - bounds.Bottom) / 2.0f;
            window.SetPosition(Canvas.Width / 2.0f - halfSizeX, Canvas.Height / 2.0f + halfSizeY);
        }
        
        // Event handlers
        public void Resize(Matrix4 projMatrix, int width, int height)
        {
            _renderer.Resize(projMatrix, width, height);
            Canvas.SetSize(width, height);
        }

        public void KeyDown(object s, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _gameWindow.Exit();
            }

            _input.ProcessKeyDown(e);
        }
        
        public void KeyUp(object s, KeyboardKeyEventArgs e)
        {
            _input.ProcessKeyUp(e);
        }

        public void MouseDown(object s, MouseButtonEventArgs args)
        {
            _input.ProcessMouseMessage(args);
        }

        public void MouseUp(object s, MouseButtonEventArgs args)
        {
            _input.ProcessMouseMessage(args);
        }

        public void MouseMove(object s, MouseMoveEventArgs args)
        {
            _input.ProcessMouseMessage(args);
        }

        public void MouseWheel(object s, MouseWheelEventArgs args)
        {
            _input.ProcessMouseMessage(args);
        }

        public void CreateDebugWindow(object o)
        {
            var window = new DebugWindow(Canvas, o, Logger);
            RegisterExplorer(window);
        }

        private void CreateArrayWindowWindow(IEnumerable collection)
        {
            var window = new DebugCollectionWindow(Canvas, collection, Logger);
            RegisterExplorer(window);
        }

        private void CreateMethodInvoker(object o, MethodInfo info)
        {
            var window = new MethodInvoker(Canvas, o, info, Logger);
            RegisterExplorer(window);
        }

        private void RegisterExplorer(Explorer explorer)
        {
            explorer.OnOpenBrowser += CreateDebugWindow;
            explorer.OnOpenArrayBrowser += CreateArrayWindowWindow;
            explorer.OnOpenMethodInvoker += CreateMethodInvoker;
        }
        
        public ILoggerSink CreateLoggerSink()
        {
            return Layout.TextOutput;
        }
    }
}