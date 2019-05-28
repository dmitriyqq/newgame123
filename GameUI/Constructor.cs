using System;
using System.Numerics;
using GameModel;
using GameModel.GameObjects;
using Gwen;
using Gwen.Control;
using ModelLoader;
using OpenTK.Input;

namespace GameUI
{
    public class Constructor : Base
    {
        private readonly AssetStore _assetStore;
        private readonly UserInterface _ui;
        private readonly Map _map;
        private readonly Logger _logger;
        private readonly IRayCaster _rayCaster;
        private CollapsibleList _listBox;
        private CollapsibleCategory _actionsCategory;
        private CollapsibleCategory _assetsCategory;
        
        public Constructor(Base parent, Logger logger, AssetStore assetStore, UserInterface ui, Map map, IRayCaster rayCaster) : base(parent)
        {
            Dock = Pos.Fill;
            _assetStore = assetStore;
            _ui = ui;
            _map = map;
            _rayCaster = rayCaster;
            _logger = logger;

            _assetStore.OnAssetUpdate += CreateListBox;
            CreateListBox();
        }

        private void CreateListBox()
        {
            _listBox = new CollapsibleList(this) {Dock = Pos.Fill};

            _actionsCategory = _listBox.Add("Actions");
            _assetsCategory = _listBox.Add("Assets");
            _assetsCategory.Selected += HandleAssetSelected;

            var createButton = _actionsCategory.Add("Create");
            createButton.Clicked += HandleCreate;
            
            var deleteButton = _actionsCategory.Add("Delete");
            deleteButton.Clicked += HandleDelete;

            var editButton = _actionsCategory.Add("Edit");
            editButton.Clicked += HandleEdit;
            
            foreach (var asset in _assetStore.Assets)
            {
                CreateRow(asset);
            }

        }

        private void HandleDelete(Base control, EventArgs e)
        {
            Unselect();
        }
        
        private void HandleEdit(Base control, EventArgs e)
        {
            Unselect();
        }
        
        private void HandleCreate(Base control, EventArgs e)
        {
            _ui.Canvas.AddChild(new CreateAssetModal(_ui.Canvas, _assetStore));
            Unselect();
        }

        private void HandleAssetSelected(Base control, ItemSelectedEventArgs args)
        {
            if (!(args.SelectedItem.UserData is Asset asset)) return;

            _ui.CursorObject.Asset = asset;
            _ui.CursorObject.Active = true;
            _ui.OnMouseMove += MoveAsset;
            _ui.OnMouseDown += PlaceAsset;
        }

        private void MoveAsset(MouseMoveEventArgs e)
        {
            var (start, dir) = _rayCaster.CastRay(e.X, e.Y);
            var v =_ui.Model.Engine.IntersectMap(_map, start, dir);
            if (v.HasValue)
            {
                _ui.CursorObject.Position = v.Value + new Vector3(0.0f, 3.0f, 0.0f);
            }
        }
        
        private void PlaceAsset(MouseButtonEventArgs e)
        {
            _logger.Info($"Placing object");
            _ui.CursorObject.Active = false;
            _ui.OnMouseMove -= MoveAsset;
            _ui.OnMouseDown -= PlaceAsset;
            Unselect();

            var typeName = _ui.CursorObject.Asset.GameObjectType;
            var type = Type.GetType(typeName);
            if (type == null)
            {
                _logger.Error($"Couldn't get type {typeName}");
                return;
            }

            if (!(Activator.CreateInstance(type) is GameObject gameObject))
            {
                _logger.Error($"Couldn't create gameObject of type {typeName}");
                return;
            }

            gameObject.Asset = _ui.CursorObject.Asset;
            gameObject.Transform.Position = _ui.CursorObject.Position;
            
            _ui.Model.AddGameObject(gameObject);
        }

        private void CreateRow(Asset asset)
        {
            var row = _assetsCategory.Add(asset?.Name ?? "No name");
            row.UserData = asset;
            
        }

        private void Unselect()
        {
            _listBox.UnselectAll();
        }
    }
}