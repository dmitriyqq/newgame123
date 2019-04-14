
using System;
using System.Drawing;
using Gwen.Control;
using Gwen.Control.Property;
using OpenTK;
using OpenTK.Input;
using Color = System.Drawing.Color;
using Font = Gwen.Font;

namespace GameUI
{
    public class UserInterface
    {
        public int Width { get; set; }

        public int Height { get; set; }

        private Canvas canvas;

        private Gwen.Renderer.OpenTK renderer;

        private Gwen.Input.OpenTK input;

        private bool altDown;
        
        public UserInterface(int width, int height, GameWindow window)
        {
            Width = width;
            Height = height;
            
            renderer = new Gwen.Renderer.OpenTK();
            var skin = new Gwen.Skin.TexturedBase(renderer, "DefaultSkin.png");

            skin.DefaultFont = new Font(renderer, "Hack", 10);
            canvas = new Canvas(skin);
            canvas.SetSize (Width, Height);
            canvas.ShouldDrawBackground = false;
            var c = Color.FromArgb(255, 0, 0, 0);
            canvas.BackgroundColor = c;
//            canvas.KeyboardInputEnabled = true;

            input = new Gwen.Input.OpenTK(window);
            input.Initialize(canvas);
            
            window.KeyDown += Keyboard_KeyDown;
            window.KeyUp += Keyboard_KeyUp;

            window.MouseDown += Mouse_ButtonDown;
            window.MouseUp += Mouse_ButtonUp;
            window.MouseMove += Mouse_Move;
            window.MouseWheel += Mouse_Wheel;  

            var w = new WindowControl(canvas, "Hello world", true);
//            canvas.AddChild(window);

            var text = new Button(w);
            text.Text = "My text";
            text.TextColor = Color.Wheat;
            text.UpdateColors();
            w.AddChild(text);
            w.SetSize(200, 200);
            w.Show();
//            var button = new Button(canvas);
//            button.Text = "My button";
//            button.UpdateColors();
//            window.AddChild(text);
        }
        
        void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                throw new Exception();
            }
            
            if (e.Key == Key.AltLeft)
            {
                altDown = true;
            }

            input.ProcessKeyDown(e);
        }
        
        void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            altDown = false;
            input.ProcessKeyUp(e);
        }

        void Mouse_ButtonDown(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_ButtonUp(object sender, MouseButtonEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_Move(object sender, MouseMoveEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        void Mouse_Wheel(object sender, MouseWheelEventArgs args)
        {
            input.ProcessMouseMessage(args);
        }

        public void Draw()
        {
            canvas.RenderCanvas();
            
        }

        public void Update(float deltaTime)
        {
            renderer.Update(deltaTime);
        }

        public void Resize(Matrix4 projMatrix, int width, int height)
        {
            Width = width;
            Height = height;
            renderer.Resize(projMatrix, width, height);
            canvas.SetSize(width, height);
        }
    }
}