using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Gwen.Control;

namespace GameUI
{
    public abstract class Explorer : WindowControl
    {
        protected Explorer(Base parent) : base(parent)
        {
        }
        
        public Action<object> OnOpenBrowser;
        
        public Action<IEnumerable> OnOpenArrayBrowser;
        
        public Action<object, MethodInfo> OnOpenMethodInvoker;
        
        protected virtual void HandleClassClick(Base control, EventArgs args)
        {
            var o = control.UserData;
            OnOpenBrowser.Invoke(o);
        }

        protected virtual void HandleArrayClick(Base control, EventArgs args)
        {
            var o = control.UserData;

            if (o is IEnumerable collection)
            {
                OnOpenArrayBrowser.Invoke(collection);                
            }
            else
            {
                throw new ArgumentException("Should be collection");
            }
        }

        protected virtual void HandleOpenMethodInvoker(Base control, EventArgs e)
        {
            var o = control.UserData;

            if (o is ValueTuple<object, MethodInfo> p)
            {
                var (obj, method) = p;
                OnOpenMethodInvoker.Invoke(obj, method);    
            }
            else
            {
                throw new ArgumentException("should be method info");
            }
            
        }
    }
}