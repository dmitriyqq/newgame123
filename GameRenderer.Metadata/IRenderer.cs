using ModelLoader;

namespace GameRenderer
{
    public interface IRenderer
    {
        IDrawable AddDrawable(Asset asset);

        void RemoveDrawable(IDrawable drawable);
    }
}