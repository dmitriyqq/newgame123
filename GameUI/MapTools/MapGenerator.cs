using System;
using GameModel;
using GameModel.GameObjects;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class MapGenerator : WindowControl
    {
        public event Action OnMapGeneration;

        private Map map;

        private Label octaveLabel;
        
        private Label persistenceLabel;

        private HorizontalSlider octaveSlider;
        
        private HorizontalSlider persistenceSlider;

        private Button button;

        private Logger logger;
        
        public MapGenerator(Base parent, Map map, Logger logger) : base(parent)
        {
            this.logger = logger;
            this.map = map;

            octaveLabel = new Label(this) {Text = map.Octaves.ToString(), Dock = Pos.Top};
            octaveSlider = new HorizontalSlider(this) {Dock = Pos.Top, Max = 50.0f, Min = 0.0f, Value = map.Octaves, NotchCount = 10, Height = 40};
            persistenceLabel = new Label(this) {Text = map.Persistence.ToString(), Dock = Pos.Top};
            persistenceSlider = new HorizontalSlider(this) {Dock = Pos.Top, Max = 50.0f, Min = 0.0f, Value = map.Persistence, NotchCount = 10, Height = 40};
            
            button = new Button(this){Dock = Pos.Top, Text = "GenerateMap"};
            button.Clicked += HandleClick;
            
            logger.Info($"Creating map generation window Persistence: {map.Octaves}, Octaves: {map.Persistence}");

            octaveSlider.ValueChanged += HandleChange;
            persistenceSlider.ValueChanged += HandleChange;

            UpdateLabels();
            Title = "Map Generator";
            SetSize(300, 200);
        }

        private void UpdateLabels()
        {
            octaveLabel.Text = $"Octaves: {((int) octaveSlider.Value).ToString()}";
            persistenceLabel.Text = $"Persistence {persistenceSlider.Value.ToString()}";
        }

        private void HandleClick(Base control, EventArgs e)
        {
            map.Octaves = (int) octaveSlider.Value;
            map.Persistence = persistenceSlider.Value;
            
            logger.Info($"Generating map Persistence: {map.Octaves}, Octaves: {map.Persistence}");
            map.CreateMap();
            OnMapGeneration?.Invoke();
        }
        
        private void HandleChange(Base control, EventArgs e)
        {
            UpdateLabels();
        }

    }
}