using System;
using Gwen;
using Gwen.Control;

namespace GameUI.AssetViewers
{
    public class AssetTypeSelector : WindowControl
    {
        private static string[] _assetTypes =
        {
            "Static",
            "Animated",
        };

        private readonly UserInterface _ui;
        private readonly string _inputtedName;
        private ComboBox _assetType;
        private Button _createButton;

        public AssetTypeSelector(Base parent, UserInterface ui, string inputtedName) : base(parent)
        {
            _ui = ui;
            _inputtedName = inputtedName;
            
            CreateLayout();
        }

        private void CreateLayout()
        {
            SetSize(300, 80);

            _assetType = new ComboBox(this) {Text = "Asset Type", Dock = Pos.Left};

            foreach (var assetType in _assetTypes)
            {
                _assetType.AddItem(assetType, assetType);
            }
            
            _createButton = new Button(this) { Text = "Create", Dock = Pos.Left};
            _createButton.Clicked += HandleSelected;
            
            _ui.SetCenter(this);
        }

        private void HandleSelected(Base control, EventArgs args)
        {
            _ui.Canvas.AddChild(new AssetViewer(_ui.Canvas, _ui, _inputtedName, _assetType.SelectedItem.Name));
            Close();
        }
    }
}