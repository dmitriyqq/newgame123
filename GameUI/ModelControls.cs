using System;
using GameModel;
using Gwen;
using Gwen.Control;
using OpenTK.Input;

namespace GameUI
{
    public class ModelControls : Base
    {
        private readonly Label _ticksLabel;
        private readonly Label _timeLabel;
        private readonly Label _objectsLabel;
        private readonly Button _picker;
        private readonly IRayCaster _rayCaster;
        private readonly UserInterface _ui;

        private Model _model;
        private ToggleButton _toggle;
        
        public ModelControls(Base parent, IGameLoop loop,  UserInterface ui, IRayCaster rayCaster) : base(parent)
        {
            _ui = ui;
            _rayCaster = rayCaster;
            _toggle = new ToggleButton(this, loop.IsPlaying, "Stop", "Play"){ Dock = Pos.Top};

            _ui.ModelReload += HandleModelReload;
            HandleModelReload(ui.Model);

            _ticksLabel = new Label(this) {Dock = Pos.Top};
            _timeLabel = new Label(this) {Dock = Pos.Top};
            _objectsLabel = new Label(this) {Dock = Pos.Top};

            _picker = new Button(this) {Text = "Pick Object", Dock = Pos.Top};
            _picker.Clicked += EnableSelector;
            
            UpdateLabels();
        }

        private void HandleModelReload(Model model)
        {
            _model = model;
        }

        private void EnableSelector(Base control, EventArgs e)
        {
            _ui.Renderer.MouseDown += Select;
        }
        
        private void DisableSelector()
        {
            _ui.Renderer.MouseDown -= Select;
        }

        private void Select(object s, MouseButtonEventArgs e)
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