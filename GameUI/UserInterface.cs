using System;
using System.Collections;
using System.Linq;
using Gwen.Control;
using OpenTK;
using OpenTK.Input;
using Font = Gwen.Font;
using System.Reflection;
using GameModel;
using Key = OpenTK.Input.Key;


namespace GameUI
{
    public class UserInterface
    {
        private Canvas _canvas;
        private Gwen.Renderer.OpenTK _renderer;
        private Gwen.Input.OpenTK _input;
        private Menu _menu;
        public event Action<MouseButtonEventArgs> OnMouseDown;
        public event Action<MouseMoveEventArgs> OnMouseMove;
        
        public InterfaceLayout Layout { get; private set; }
        public Logger Logger { get; }
        public CursorObject CursorObject { get; }
        public Model Model { get; }

        public UserInterface(GameWindow window, Model model)
        {
            Model = model;
            Logger = new Logger("UI");
            CursorObject = new CursorObject();

            Logger.Info("Created ui");
            try
            {
                CreateCanvas(window);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void CreateCanvas(GameWindow window)
        {
            _renderer = new Gwen.Renderer.OpenTK();
            var skin = new Gwen.Skin.TexturedBase(_renderer, "DefaultSkin.png"){
                DefaultFont = new Font(_renderer, "Hack", 9)
            };

            _canvas = new Canvas(skin) {ShouldDrawBackground = false};
            _canvas.SetSize (window.Width, window.Height);

            _input = new Gwen.Input.OpenTK(window);
            _input.Initialize(_canvas);
        }
 
        public void Draw()
        {
            _canvas.RenderCanvas();
        }

        public void Update(float deltaTime)
        {
            _renderer.Update(deltaTime);
        }
        public void AddRendererMenu(IGameLoop renderer)
        {
            _menu = new MenuStrip(_canvas);

            var debugMenu = _menu.AddItem("Debug");
            debugMenu.Menu.AddItem("Model").SetAction((control, eventArgs) => CreateDebugWindow(Model));
            debugMenu.Menu.AddItem("Renderer").SetAction((control, eventArgs) => CreateDebugWindow(renderer));
            debugMenu.Menu.AddItem("Interface").SetAction((control, eventArgs) => CreateDebugWindow(this));
            debugMenu.Menu.AddItem("Settings").SetAction((control, eventArgs) => {CreateDebugWindow(new {settings="settings",x = 200, y = 300});});

            var tools = _menu.AddItem("Tools");
            tools.Menu.AddItem("Map Generator").SetAction((control, eventArgs) => { CreateMapGeneratorWindow();});
            
            Layout = new InterfaceLayout(_canvas, Logger, renderer, Model, this);
        }

        private void CreateMapGeneratorWindow()
        {
            // Search for map game object
            var mapGameObject = Model.GameObjects.FirstOrDefault(go => go is Map);
            if (mapGameObject is Map map)
            {
                var window = new MapGenerator(_canvas,map , Logger);
            }
        }
        
        // Event handlers
        public void Resize(Matrix4 projMatrix, int width, int height)
        {
            _renderer.Resize(projMatrix, width, height);
            _canvas.SetSize(width, height);
        }

        public bool KeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                throw new Exception();
            }

            return _input.ProcessKeyDown(e);
        }
        
        public bool KeyUp(KeyboardKeyEventArgs e)
        {
            return _input.ProcessKeyUp(e);
        }

        public bool MouseDown(MouseButtonEventArgs args)
        {
            OnMouseDown?.Invoke(args);
            return _input.ProcessMouseMessage(args);
        }

        public bool MouseUp(MouseButtonEventArgs args)
        {
            return _input.ProcessMouseMessage(args);
        }

        public bool MouseMove(MouseMoveEventArgs args)
        {
            OnMouseMove?.Invoke(args);
            return _input.ProcessMouseMessage(args);
        }

        public bool MouseWheel(MouseWheelEventArgs args)
        {
            return _input.ProcessMouseMessage(args);
        }

        public void CreateDebugWindow(object o)
        {
            var window = new DebugWindow(_canvas, o, Logger);
            RegisterExplorer(window);
        }
        
        public void CreateArrayWindowWindow(IEnumerable collection)
        {
            var window = new DebugCollectionWindow(_canvas, collection, Logger);
            RegisterExplorer(window);
        }

        public void CreateMethodInvoker(object o, MethodInfo info)
        {
            var window = new MethodInvoker(_canvas, o, info, Logger);
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