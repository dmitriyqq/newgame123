using System;
using System.Linq;
using GameRenderer.Metadata.Assets;
using GameUI.AssetViewers;
using Gwen;
using Gwen.Control;
using ModelLoader;

namespace GameUI
{
    public class CreateAssetModal : WindowControl
    {
        private readonly AssetStore _store;
        private readonly UserInterface _ui;

        private Button _editButton;
        private TextBox _assetName;
        private ComboBox _assetsList;
        
        public CreateAssetModal(Base parent, UserInterface ui, AssetStore store) : base(parent)
        {
            _store = store;
            _ui = ui;

            Title = "Asset Selector";
            CreateLayout();
        }

        private void CreateLayout()
        {
            SetSize(300, 200);

            AddChild(new Label(this) {Text = "Enter name or select existing from list", Dock = Pos.Top});

            _assetName = new TextBox(this) {Text = "Name", Dock = Pos.Top, Margin = Margin.One};

            _assetsList = new ComboBox(this) {Text = "Asset", Dock = Pos.Top, Margin = Margin.One};
            UpdateAssets();
            _assetsList.ItemSelected += HandleEdit;

            _editButton = new Button(this) {Text = "Open", Dock = Pos.Bottom, Margin = Margin.One};
            _editButton.Clicked += HandleOpen;

            _ui.SetCenter(this);
        }

        private void UpdateAssets()
        {
            _assetsList.DeleteAll();
            foreach (var asset in _store.Assets)
            {
                var item = _assetsList.AddItem(asset.Name);
                item.UserData = asset;
            }
        }

        private void OpenAssetViewer(Asset asset)
        {
            var widget = asset is StaticModelAsset ? "Static" : "Animated"; 
            
            _ui.Canvas.AddChild(new AssetViewer(_ui.Canvas, _ui, asset.Name, widget));
        }

        private void OpenAssetTypeSelector()
        {
            _ui.Canvas.AddChild(new AssetTypeSelector(_ui.Canvas, _ui, _assetName.Name));
        }
        
        private void HandleEdit(Base control, EventArgs e)
        {
            if (control.UserData is Asset asset)
            {
                OpenAssetViewer(asset);
                Close();
            }
        }

        private void HandleOpen(Base control, EventArgs e)
        {
            OpenAssetTypeSelector();
            Close();
        }
    }
}