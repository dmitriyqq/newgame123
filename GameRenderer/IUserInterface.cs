using OpenTK;
using OpenTK.Input;

namespace GameRenderer
{
    public interface IUserInterface
    {
        void Draw();
        void Update(float deltaTime);
        void Resize(Matrix4 projMatrix, int width, int height);
        void KeyDown(object s, KeyboardKeyEventArgs e);
        void KeyUp(object s, KeyboardKeyEventArgs e);
        void MouseDown(object s, MouseButtonEventArgs args);
        void MouseUp(object s, MouseButtonEventArgs args);
        void MouseMove(object s, MouseMoveEventArgs args);
        void MouseWheel(object s, MouseWheelEventArgs args);
    }
}