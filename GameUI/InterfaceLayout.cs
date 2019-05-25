using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class InterfaceLayout : DockBase
    {
        public EventView TextOutput { get; private set; }
        public ModelControls ModelControls { get; private set; }
        
        public InterfaceLayout(Base parent, Logger logger, IGameLoop renderer, Model model) : base(parent)
        {
            Dock = Pos.Fill;
            TextOutput = new EventView(BottomDock);
            BottomDock.TabControl.AddPage("Output", TextOutput);
            BottomDock.Height = 200;
            
            ModelControls = new ModelControls(RightDock, renderer, model);
            RightDock.TabControl.AddPage("Model Controls", ModelControls);
            RightDock.Width = 150;
        }
    }
}