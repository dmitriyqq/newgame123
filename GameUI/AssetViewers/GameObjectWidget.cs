using System;
using System.Collections.Generic;
using GameRenderer.Metadata.Assets;
using Gwen;
using Gwen.Control;

namespace GameUI.AssetViewers
{
    public class GameObjectWidget : Base
    {
        private readonly IEnumerable<Type> _gameObjectTypes;
        private ComboBox _gameObjectType;
        private LabeledCheckBox _static;
        
        public GameObjectWidget(Base parent, IEnumerable<Type> gameObjectTypes) : base(parent)
        {
            Dock = Pos.Fill;
            _gameObjectTypes = gameObjectTypes;
            CreateLayout();
        }

        private void CreateLayout()
        {
            _gameObjectType = new ComboBox(this) { Dock = Pos.Top, Text = "GameObject Type"};

            foreach (var type in _gameObjectTypes)
            {
                _gameObjectType.AddItem(type.Name);
            }
            
            _static = new LabeledCheckBox(this) {Dock = Pos.Top, Text = "Static"};
        }
    }
}