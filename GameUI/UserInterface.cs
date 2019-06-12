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
using Key = OpenTK.Input.Key;


namespace GameUI
{
    public class UserInterface : IUserInterface
    {
        private Gwen.Renderer.OpenTK _renderer;
        private Gwen.Input.OpenTK _input;
        private Menu _menu;
        public InterfaceLayout Layout { get; private set; }
        public Logger Logger { get; }
        public Model Model { get; }
        public Canvas Canvas { get; private set; }
        public GameWindow Window { get; private set; }
        
        public UserInterface(GameWindow window, Model model)
        {
            Window = window;
            Model = model;
            Logger = new Logger("UI");
            Logger.Info("Created ui");

            try
            {
                CreateCanvas(window);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            
            Window.KeyUp += KeyUp;
            Window.MouseUp += MouseUp;
            Window.MouseDown += MouseDown;
            Window.MouseMove += MouseMove;
            Window.MouseWheel += MouseWheel;
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
            debugMenu.Menu.AddItem("Settings").SetAction((control, eventArgs) => {CreateDebugWindow(new {settings="settings",x = 200, y = 300});});

            var tools = _menu.AddItem("Tools");
            tools.Menu.AddItem("Map Generator").SetAction((control, eventArgs) => { CreateMapGeneratorWindow();});

            Layout = new InterfaceLayout(Canvas, Logger, renderer, Model, this);
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
                throw new Exception();
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
        
        public void CreateArrayWindowWindow(IEnumerable collection)
        {
            var window = new DebugCollectionWindow(Canvas, collection, Logger);
            RegisterExplorer(window);
        }

        public void CreateMethodInvoker(object o, MethodInfo info)
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