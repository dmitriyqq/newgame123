using System;
using System.Linq;
using System.Reflection;
using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class MethodInvoker : Explorer
    {
        private object obj;

        private MethodInfo method;

        private string[] parameters;

        private Logger logger;
        public MethodInvoker(Base parent, object obj, MethodInfo method, Logger logger) : base(parent)
        {
            this.obj = obj;
            this.method = method;
            this.logger = logger;
            
            Title = $"Invoke {method.Name}";
            
            AddControls();
        }

        private void AddControls()
        {

            var methodParameters = method.GetParameters();
            var numberOfParams = methodParameters.Length;
            parameters = new string[numberOfParams];


            var props = new Properties(this) {Dock = Pos.Top};
            
            var button = new Button(this) {Text = "Call method", Dock = Pos.Bottom};
            
            button.Clicked += HandleInvoke;

            
            foreach (var param in method.GetParameters())
            {
                object value = null;

                if (param.HasDefaultValue)
                {
                    value = param.DefaultValue;
                }
                
                PropertyRow row;
                if (param.ParameterType == typeof(bool))
                {
                    row = props.Add(param.Name, new Gwen.Control.Property.Color(props), value?.ToString() ?? "0");
                }
                else
                {
                    row = props.Add(param.Name, value?.ToString() ?? "null");
                }

                row.UserData = param;
                row.ValueChanged += HandleValueChanged;
            }

            SetSize(400, Math.Max(400, method.GetParameters().Length * 20));
        }

        private void HandleValueChanged(Base control, EventArgs e)
        {
            var u = control.UserData;
            var row = control as PropertyRow;

            if (u is ParameterInfo param)
            {
                parameters[param.Position] = row.Value;
            }
            else
            {
                throw new ArgumentException("Should be param info");
            }
        }

        private void HandleInvoke(Base control, EventArgs ev)
        {
            var paramsList = new object[method.GetParameters().Length];

            try
            {
                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType == typeof(string))
                    {
                        paramsList[param.Position] = parameters[param.Position];
                    }
                    else if (param.ParameterType == typeof(int))
                    {
                        paramsList[param.Position] = int.Parse(parameters[param.Position]);
                    }
                    else if (param.ParameterType == typeof(float))
                    {
                        paramsList[param.Position] = float.Parse(parameters[param.Position]);
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        paramsList[param.Position] = bool.Parse(parameters[param.Position]);
                    }
                    else
                    {
                        paramsList[param.Position] = null;
                    }
                }

                if (paramsList.Length > 0)
                {
                    logger.Info($"Invoking method {method.Name} with params {string.Join(",", paramsList.Select(p =>p?.ToString() ?? "null"))}");    
                }
                else
                {
                    logger.Info($"Invoking method {method.Name}");
                }
                
                var result = method.Invoke(obj, paramsList);
                if (result != null)
                {
                    OnOpenBrowser(result);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}