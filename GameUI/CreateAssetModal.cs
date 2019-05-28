using System;
using Gwen;
using Gwen.Control;
using ModelLoader;

namespace GameUI
{
    public class CreateAssetModal : WindowControl
    {
        private TextBox _name;
        private ComboBox _type;
        private AssetStore _store;
        private Button _saveButton;
        
        public CreateAssetModal(Base parent, AssetStore store) : base(parent)
        {
            SetSize(300, 300);
            Title = "Create Asset";

            _name = new TextBox(this) {Text = "Name", Dock = Pos.Top, Margin = Margin.One};
            _type = new ComboBox(this) {Text = "Type", Dock = Pos.Top, Margin = Margin.One};
            _type.AddItem("TestType");
            _saveButton = new Button(this) {Text = "Save", Dock = Pos.Bottom, Margin = Margin.One};
            _saveButton.Clicked += HandleSave;
            _store = store;
        }

        private void HandleSave(Base control, EventArgs e)
        {
            var asset = new Asset
            {
                Name = _name.Text,
                GameObjectType = _type.SelectedItem.Text
            };
            _store.AddAsset(asset);
        }
        
        
    }
}