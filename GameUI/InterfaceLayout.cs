using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class InterfaceLayout : DockBase
    {
        public EventView TextOutput { get; private set; }
        
        public InterfaceLayout(Base parent, Logger logger) : base(parent)
        {
            Dock = Pos.Fill;
            TextOutput = new EventView(BottomDock);
            BottomDock.TabControl.AddPage("Output", TextOutput);
            BottomDock.Height = 200;
        }
    }
}