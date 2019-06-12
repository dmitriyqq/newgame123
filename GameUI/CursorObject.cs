
using System;
using System.Numerics;
using GameRenderer.Metadata.Assets;
using ModelLoader;

namespace GameUI
{
    public class CursorObject
    {
        private Asset _asset;
        private Vector3 _position;
        private bool _active;
        public Asset Asset
        {
            get => _asset;
            set {  _asset = value; OnUpdateAsset?.Invoke(); }
        }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                if (_active)
                {
                    OnShow?.Invoke();
                }
                else
                {
                    OnHide?.Invoke();
                }
                
            }
        }

        public Vector3 Position
        {
            get => _position;
            set {  _position = value; OnUpdatePosition?.Invoke(); }
        }
        public event Action OnShow;
        public event Action OnHide;
        public event Action OnUpdateAsset;
        public event Action OnUpdatePosition;
    }
}