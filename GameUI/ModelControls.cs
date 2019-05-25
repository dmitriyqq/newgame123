using System.Drawing;
using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class ModelControls : Base
    {
        private Model model; 

        private ToggleButton _toggle;
        private Label ticksLabel;
        private Label timeLabel;
        private Label objectsLabel;
        
        public ModelControls(Base parent, IGameLoop loop, Model model) : base(parent)
        {
            _toggle = new ToggleButton(this, loop.IsPlaying, "Stop", "Play"){ Dock = Pos.Top};

            ticksLabel = new Label(this) { Dock = Pos.Top};
            timeLabel = new Label(this) { Dock = Pos.Top};
            objectsLabel = new Label(this) { Dock = Pos.Top};
            
            this.model = model;
            UpdateLabels();
        }

        public override void Think()
        {
            base.Think();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            ticksLabel.SetText($"{model.Ticks} ticks");
            timeLabel.SetText($"{model.Time} ms");
            objectsLabel.SetText($"{model.NumObjects} objects");
        }
    }
}