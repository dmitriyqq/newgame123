using System;
using System.Linq;
using GameModel;
using GameModel.GameObjects;
using Gwen;
using Gwen.Control;
using OpenTK.Input;

namespace GameUI
{
    public class MapTools : Base
    {
        private readonly Button _select;
        private readonly ComboBox _tool;
        private readonly UserInterface _ui;
        private readonly IRayCaster _rayCaster;
        
        private readonly Slider _brushSize;
        private readonly ComboBox _brushType;

        private Map _map;
        private Model _model;

        private Brush _selectedBrush;
        private Tool _selectedTool;
        
        public MapTools(Base parent, UserInterface ui, IRayCaster rayCaster) : base(parent)
        {
            Dock = Pos.Fill;
            _rayCaster = rayCaster;

            _select = new Button(this) {Dock = Pos.Top, Text = "Select"};
            _select.Clicked += EnableTool;

            _ui = ui;
            _ui.ModelReload += HandleModelReload;
            HandleModelReload(_ui.Model);

            _selectedBrush = new RectangleBrush();
            _selectedTool = new ToolUp();
            
            _tool = new ComboBox(this) {Dock = Pos.Top, Margin = Margin.Five};
            _tool.AddItem("Painter", "", new ToolUp());
            _tool.AddItem("Up", "", new ToolUp());
            _tool.AddItem("Down", "", new ToolDown());
            _tool.AddItem("Adjust", "", new ToolAverage());
            _tool.ItemSelected += SelectTool;
            
            _brushType = new ComboBox(this) {Dock = Pos.Top, Margin = Margin.Five};
            _brushType.AddItem("Pointer", "", new Pointer());
            _brushType.AddItem("Circle", "", new CircleBrush());
            _brushType.AddItem("Rectangle", "", new RectangleBrush());
            _brushType.ItemSelected += SelectBrush;
            
            _brushSize = new HorizontalSlider(this) { Dock = Pos.Top, Value = 3.0f, Min = 0.0f, Max = 10.0f, SnapToNotches = true, NotchCount = 11, DrawNotches = true};
            _brushSize.ValueChanged += HandleChangeSize;
        }

        private void HandleModelReload(Model model)
        {
            _map = null;
            _select.Disable();
            
            if (_model != null)
            {
                _model.OnAddGameObject -= UpdateMap;
                _model.OnRemoveGameObject -= HandleRemove;
            }
            
            _model = model;
            _model.OnAddGameObject += UpdateMap;
            _model.OnRemoveGameObject += HandleRemove;

            var map = _model.GameObjects.FirstOrDefault(o => o is Map) as Map;
            UpdateMap(map);
        }

        private void UpdateMap(GameObject gameObject)
        {
            if (gameObject is Map map)
            {
                _select.Enable();
                _map = map;
            }
        }

        private void HandleRemove(GameObject gameObject)
        {
            if (gameObject is Map)
            {
                _map = null;
                _select.Disable();
            }
        }
        
        private void SelectBrush(Base control, ItemSelectedEventArgs e)
        {
            if (e.SelectedItem.UserData is Brush brush)
            {
                _selectedBrush = brush;
                _selectedBrush.Size = (int) _brushSize.Value;
            }
        }

        private void HandleChangeSize(Base control, EventArgs e)
        {
            _selectedBrush.Size = (int) _brushSize.Value;
        }
        
        private void SelectTool(Base control, ItemSelectedEventArgs e)
        {
            if (e.SelectedItem.UserData is Tool tool)
            {
                _selectedTool = tool;
            }
        }
        
        private void EnableTool(Base control, EventArgs e)
        {
            _ui.Renderer.MouseDown += HandleStartDraw;
            _ui.Renderer.MouseUp += Stop;
        }

        private void Draw(object s, MouseMoveEventArgs e)
        {
            var (start, dir) = _rayCaster.CastRay(e.X, e.Y);
            var hit = _model.Engine.IntersectMap(_map, start, dir, out var normal);
            
            if (hit.HasValue)
            {
                _selectedBrush.Select(_map, hit.Value, _selectedTool);
            }
            
            _map.Update();
        }
        
        private void Stop(object s, MouseButtonEventArgs e)
        {
            _ui.Renderer.MouseDown -= HandleStartDraw;
            _ui.Renderer.MouseMove -= Draw;
            _ui.Renderer.MouseUp -= Stop;
            _map.BatchUpdate();
        }

        private void HandleStartDraw(object s, MouseButtonEventArgs e)
        {
            _ui.Renderer.MouseMove += Draw;
        }
    }
}