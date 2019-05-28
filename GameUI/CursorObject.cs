
using System;
using System.Numerics;
using ModelLoader;

namespace GameUI
{
    public class CursorObject
    {
        public Asset Asset { get; set; }
        public bool Active { get; set; }
        public Vector3 Position { get; set; }
        public event Action OnShow;
        public event Action OnHide;
        public event Action OnUpdateAsset;

        public void Show()
        {
            Active = true;
            OnShow?.Invoke();
        }

        public void Hide()
        {
            Active = false;
            OnHide?.Invoke();
        }

        public void UpdateAsset(Asset asset)
        {
            OnUpdateAsset?.Invoke();
        }
    }
}