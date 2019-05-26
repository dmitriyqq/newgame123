using System;
using GameModel;
using Gwen;
using Gwen.Control;
using OpenTK.Input;

namespace GameUI
{
    public class ModelControls : Base
    {
        private ToggleButton _toggle;
        private readonly Label _ticksLabel;
        private readonly Label _timeLabel;
        private readonly Label _objectsLabel;
        private readonly Button _picker;
        private readonly IRayCaster _rayCaster;
        private readonly UserInterface _ui;
        private readonly Model _model;
        
        
        public ModelControls(Base parent, IGameLoop loop, Model model, UserInterface ui, IRayCaster rayCaster) : base(parent)
        {
            _ui = ui;
            _rayCaster = rayCaster;
            _model = model;
            _toggle = new ToggleButton(this, loop.IsPlaying, "Stop", "Play"){ Dock = Pos.Top};

            _ticksLabel = new Label(this) { Dock = Pos.Top};
            _timeLabel = new Label(this) { Dock = Pos.Top};
            _objectsLabel = new Label(this) { Dock = Pos.Top};
            _picker = new Button(this) {Text = "Pick Object", Dock = Pos.Top};
            _picker.Clicked += EnableSelector;
            
            UpdateLabels();
        }

        private void EnableSelector(Base control, EventArgs e)
        {
            _ui.OnMouseDown += Select;
        }
        
        private void DisableSelector()
        {
            _ui.OnMouseDown -= Select;
        }

        private void Select(MouseButtonEventArgs e)
        {
            DisableSelector();
            var (start, end) = _rayCaster.CastRay(e.X, e.Y);
            var gameObjectHandle = _model.Engine.IntersectGameObjects(start, end);

            if (!gameObjectHandle.HasValue) return;

            var gameObject = _model.GameObjectByHandle(gameObjectHandle.Value);
            _ui.CreateDebugWindow(gameObject);
        }
        
        public override void Think()
        {
            base.Think();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _ticksLabel.SetText($"{_model.Ticks} ticks");
            _timeLabel.SetText($"{_model.Time} ms");
            _objectsLabel.SetText($"{_model.NumObjects} objects");
        }
    }
}