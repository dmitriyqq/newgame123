
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Gwen.Control;
using Gwen.Control.Property;
using OpenTK;
using OpenTK.Input;
using Color = System.Drawing.Color;
using Font = Gwen.Font;
using System.Reflection;
using GameModel;
using Gwen;
using Base = Gwen.Control.Base;
using Key = OpenTK.Input.Key;


namespace GameUI
{
    public class UserInterface
    {
        private Canvas canvas;

        private Gwen.Renderer.OpenTK renderer;

        private Gwen.Input.OpenTK input;

        private bool altDown;

        private Menu menu;

        private InterfaceLayout layout;
        
        public Logger Logger { get; private set; }

        public event Action OnMapGeneration;

        private Model model;

        public UserInterface(GameWindow window, Model model)
        {
            this.model = model;
            Logger = new Logger("UI");
            Logger.Info("Created ui");
            
            renderer = new Gwen.Renderer.OpenTK();
            var skin = new Gwen.Skin.TexturedBase(renderer, "DefaultSkin.png");
            skin.DefaultFont = new Font(renderer, "Hack", 9);

            canvas = new Canvas(skin);
            canvas.SetSize (window.Width, window.Height);
            canvas.ShouldDrawBackground = false;

            input = new Gwen.Input.OpenTK(window);
            input.Initialize(canvas);
            
            layout = new InterfaceLayout(canvas, Logger);
        }
 
        public void Draw()
        {
            canvas.RenderCanvas();
        }

        public void Update(float deltaTime)
        {
            renderer.Update(deltaTime);
        }

        public void AddMenu(object model, object renderer)
        {
            menu = new MenuStrip(canvas);
            var debugMenu = menu.AddItem("Debug");

            debugMenu.Menu.AddItem("Model").SetAction((control, eventArgs) => CreateDebugWindow(model));
            debugMenu.Menu.AddItem("Renderer").SetAction((control, eventArgs) => CreateDebugWindow(renderer));
            debugMenu.Menu.AddItem("Interface").SetAction((control, eventArgs) => CreateDebugWindow(this));
            debugMenu.Menu.AddItem("Settings").SetAction((control, eventArgs) => {CreateDebugWindow(new {settings="settings",x = 200, y = 300});});

            var tools = menu.AddItem("Tools");
            tools.Menu.AddItem("Map Generator").SetAction((control, eventArgs) => { createMapGeneratorWindow();});
        }

        private void createMapGeneratorWindow()
        {
            var window = new MapGenerator(canvas, model.Map, Logger);
            window.OnMapGeneration += OnMapGeneration;
        }
        
        // Event handlers
        public void Resize(Matrix4 projMatrix, int width, int height)
        {
            renderer.Resize(projMatrix, width, height);
            canvas.SetSize(width, height);
        }

        public bool KeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                throw new Exception();
            }
            
            if (e.Key == Key.AltLeft)
            {
                altDown = true;
            }

            return input.ProcessKeyDown(e);
        }
        
        public bool KeyUp(KeyboardKeyEventArgs e)
        {
            altDown = false;
            return input.ProcessKeyUp(e);
        }

        public bool MouseDown(MouseButtonEventArgs args)
        {
            return input.ProcessMouseMessage(args);
        }

        public bool MouseUp(MouseButtonEventArgs args)
        {
            return input.ProcessMouseMessage(args);
        }

        public bool MouseMove(MouseMoveEventArgs args)
        {
            return input.ProcessMouseMessage(args);
        }

        public bool MouseWheel(MouseWheelEventArgs args)
        {
            return input.ProcessMouseMessage(args);
        }

        public void CreateDebugWindow(object o)
        {
            var window = new DebugWindow(canvas, o, Logger);
            RegisterExplorer(window);
        }
        
        public void CreateArrayWindowWindow(IEnumerable collection)
        {
            var window = new DebugCollectionWindow(canvas, collection, Logger);
            RegisterExplorer(window);
        }

        public void CreateMethodInvoker(object o, MethodInfo info)
        {
            var window = new MethodInvoker(canvas, o, info, Logger);
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
            return layout.TextOutput;
        }
    }
}