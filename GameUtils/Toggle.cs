using System;

namespace GameModel
{
    public class Toggle
    {
        private bool Value { get; set; }

        public event Action<bool> OnToggle;

        public Toggle(bool t = false)
        {
            Value = t;
        }
        
        public static implicit operator bool(Toggle v)
        {
            return v.Value;
        }

        public void ToggleMe()
        {
            Value = !Value;
            OnToggle?.Invoke(Value); 
        }
    }
}