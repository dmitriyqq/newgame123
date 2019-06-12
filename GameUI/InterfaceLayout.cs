using GameModel;
using GameModel.GameObjects;
using GameRenderer;
using Gwen;
using Gwen.Control;
using ModelLoader;
using Newtonsoft.Json.Serialization;

namespace GameUI
{
    public class InterfaceLayout : DockBase
    {
        private readonly UserInterface _ui;
        private readonly Logger _logger;
        public EventView TextOutput { get; private set; }
        public ModelControls ModelControls { get; private set; }
        public MapTools MapTools { get; private set; }
        public Constructor ObjectConstructor { get; private set; }

        public InterfaceLayout(Base parent, Logger logger, IGameLoop renderer, Model model, UserInterface ui) : base(parent)
        {
            Dock = Pos.Fill;
            _ui = ui;
            _logger = logger;
        }

        public void AddEventView()
        {
            TextOutput = new EventView(BottomDock);
            BottomDock.TabControl.AddPage("Output", TextOutput);
            BottomDock.Height = 200;
        }

        public void AddModelControls(IGameLoop renderer, Model model, IRayCaster rayCaster)
        {
            ModelControls = new ModelControls(RightDock, renderer, model, _ui, rayCaster);
            RightDock.TabControl.AddPage("Model Controls", ModelControls);
            RightDock.Width = 150;
        }

        public void AddConstructor(AssetStore assetStore, Map map, IRayCaster rayCaster, Renderer renderer)
        {
            ObjectConstructor = new Constructor(LeftDock, _logger, assetStore, _ui, map, rayCaster, renderer);
            LeftDock.TabControl.AddPage("Constructor", ObjectConstructor);
            RightDock.Width = 300;
        }
        
        public void AddMapTools(Map map, UserInterface ui, Model model, IRayCaster rayCaster)
        {
            MapTools = new MapTools(LeftDock, map, model, ui, rayCaster);
            LeftDock.TabControl.AddPage("MapTools", MapTools);
            RightDock.Width = 300;
        }
    }
}