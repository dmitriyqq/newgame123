
using System;
using System.Drawing;
using Gwen.Control;
using Gwen.Control.Property;
using OpenTK;
using OpenTK.Input;
using Color = System.Drawing.Color;
using Font = Gwen.Font;
using System.Reflection;
using Gwen;
using Key = OpenTK.Input.Key;


namespace GameUI
{
    public class UserInterface
    {
        private Canvas canvas;

        private Gwen.Renderer.OpenTK renderer;

        private Gwen.Input.OpenTK input;

        private bool altDown;

        public UserInterface(int width, int height, GameWindow window)
        {
            renderer = new Gwen.Renderer.OpenTK();

            var skin = new Gwen.Skin.TexturedBase(renderer, "DefaultSkin.png");
            skin.DefaultFont = new Font(renderer, "Hack", 9);

            canvas = new Canvas(skin);
            canvas.SetSize (width, height);
            canvas.ShouldDrawBackground = false;

            input = new Gwen.Input.OpenTK(window);
            input.Initialize(canvas);
        }
 
        public void Draw()
        {
            canvas.RenderCanvas();
        }

        public void Update(float deltaTime)
        {
            renderer.Update(deltaTime);
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

        public void CreateDebugWindow(Object o)
        {
            var fields = o.GetType().GetFields();
            
            var objectWindow = new WindowControl(canvas, "Properties " + o.GetType().Name, true);
            objectWindow.SetSize(200, 400);

            var props = new Properties(objectWindow);
            props.SetBounds(10, 10, 150, 300);
            props.SetPosition(0, 50);

            var closeButton = new Button(objectWindow) {Text = "Close"};
            closeButton.Clicked += (control, args) => objectWindow.Close();
            closeButton.SetSize(180, 50);
            closeButton.SetPosition(0, 0);
            closeButton.Alignment = Pos.Center;

            Console.WriteLine($"Total Fields {fields.Length}");

            foreach (var field in fields)
            {
                var value = field.GetValue(o).ToString();
                props.Add(field.Name, value: value);

                Console.WriteLine($"Field: {field.Name}");
            }

            props.ValueChanged += (control, args) =>
            {
                PropertyRow row = control as PropertyRow;
                
                o.GetType().GetField(row.Name).SetValue(o, row.Value);
                
                Console.WriteLine(o);
            };
        }
    }
}