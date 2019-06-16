using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class DebugWindow : Explorer
    {
        private static BindingFlags Flags => BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        private readonly Dictionary<PropertyInfo, string> _propValues = new Dictionary<PropertyInfo, string>();
        private readonly Dictionary<FieldInfo, string> _fieldValues = new Dictionary<FieldInfo, string>();
        private readonly Logger _logger;
        private readonly object _objectToExplore;
        private Properties _fieldProps;
        private TabControl _tabs;
        private Properties _propertiesProps;
        private Type Type => _objectToExplore.GetType();
        private IEnumerable<FieldInfo> Fields => Type.GetFields(Flags);
        private IEnumerable<PropertyInfo> Properties => Type.GetProperties(Flags);

        public DebugWindow(Base parent, object objectToExplore, Logger logger) : base(parent)
        {
            _objectToExplore = objectToExplore;
            _logger = logger;

            Title = $"Object {objectToExplore.GetType().Name}";
            logger.Info($"Creating window for object {objectToExplore}");

            SetSize(400, 400);
            CreateTabs();
            
            var updateButton = new Button(this) { Dock = Pos.Bottom, Text = "Set values"};
            updateButton.Clicked += HandleUpdateClick;
            
            var refreshButton = new Button(this) { Dock = Pos.Bottom, Text = "Update values"};
            refreshButton.Clicked += HandleRefreshClick;
        }

        public sealed override bool SetSize(int width, int height)
        {
            return base.SetSize(width, height);
        }

        private void HandleUpdateClick(Base control, EventArgs e)
        {
            Update();
        }
        
        private void HandleRefreshClick(Base control, EventArgs e)
        {
            Update();
        }

        private void CreateTabs()
        {
            _tabs = new TabControl(this) {Dock = Pos.Fill};
            AddFields();
            AddProperties();
            AddMethods();
        }

        private void Refresh()
        {
            RemoveChild(_tabs, true);
            CreateTabs();
        }

        private void Update()
        {
            UpdateProps();
            UpdateFields();
            Refresh();
        }
        private void UpdateProps()
        {
            try
            {
                foreach (var prop in _propValues)
                {
                    var property = prop.Key;
                    var str = prop.Value;

                    object value;

                    if (property.PropertyType == typeof(string))
                    {
                        value = str;
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        value = int.Parse(str);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        value = float.Parse(str);
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        value = str != "" && str != "0" && str != "false";
                    }
                    else
                    {
                        continue;
                    }
                    
                    _logger.Info($"Updating property {property.Name} with value {value}");
                    property.SetMethod.Invoke(_objectToExplore, new[] {value});
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            
            _propValues.Clear();
        }
        
        private void UpdateFields()
        {
            try
            {
                foreach (var prop in _fieldValues)
                {
                    var field = prop.Key;
                    var strValue = prop.Value;

                    object value;

                    if (field.FieldType == typeof(string))
                    {
                        value = strValue;
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        value = int.Parse(strValue);
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        value = float.Parse(strValue);
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        value = strValue != "" && strValue != "0" && strValue != "false";
                    }
                    else
                    {
                        continue;
                    }

                    _logger.Info($"Updating property {field.Name} with value {value}");
                    field.SetValue(_objectToExplore, value);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            
            _fieldValues.Clear();
        }

        private Properties CreateGroup(string name)
        {
            var button = _tabs.AddPage(name);
            var page = button.Page;
            
            var scroll = new ScrollControl(page);
            scroll.EnableScroll(true, true);
            scroll.Dock = Pos.Fill;
            var props = new Properties(scroll) {Dock = Pos.Fill};
            return props;
        }

        private void AddFields()
        {
            // All fields
           _fieldProps = CreateGroup("Fields");

            try
            {
                foreach (var field in Fields)
                {
                    var value = field.GetValue(_objectToExplore);

                    var row = CreateRowForObject(field.Name, value, _fieldProps);
                    row.UserData = field;
                    row.ValueChanged += HandleFieldChange;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private PropertyRow CreateRowForObject(string name, object obj, Properties props)
        {
            PropertyRow row;

            if (obj == null)
            {
                row = props.Add(name, "null");
            }
            else if (obj is string)
            {
                // raw string input
                row = props.Add(name, new Gwen.Control.Property.Text(props), obj.ToString());
            } else if (obj is int)
            {
                // int input
                row = props.Add(name, new Gwen.Control.Property.Text(props), obj.ToString());
            }
            else if (obj is float)
            {
                // float input
                row = props.Add(name, new Gwen.Control.Property.Text(props), obj.ToString());
            }
            else if (obj is bool)
            {
                // checkbox
                row = props.Add(name, new Gwen.Control.Property.Check(props), obj.ToString());
            }
            else if (obj is IEnumerable enumerable)
            {
                string value = "Enumerable";

                if (obj is ICollection collection)
                {
                    value = $"Collection[Count = {collection.Count}]";
                }
                // it is array so open in array explorer
                    
                row = props.Add(name, new Gwen.Control.Property.Text(props), value);
                row.UserData = enumerable;
                row.Clicked += HandleArrayClick;
            }
            else
            {
                // it is object so only open in another explorer
                row = props.Add(name, new Gwen.Control.Property.Text(props), obj.ToString());
                row.UserData = obj;
                row.Clicked += HandleClassClick;
            }

            return row;
        }

        protected override void HandleClassClick(Base control, EventArgs args)
        {
            var data = control.UserData;
            // Dirty hack
            if (data is FieldInfo fieldInfo)
            {
                control.UserData = fieldInfo.GetValue(_objectToExplore);
            } else if (data is PropertyInfo propertyInfo)
            {
                control.UserData = propertyInfo.GetValue(_objectToExplore);
            }
            else
            {
                _logger.Error("UserData should be field/property");
            }
            
            base.HandleClassClick(control, args);
            control.UserData = data;
        }
        
        protected override void HandleArrayClick(Base control, EventArgs args)
        {
            var data = control.UserData;

            // Dirty hack
            if (data is FieldInfo fieldInfo)
            {
                control.UserData = fieldInfo.GetValue(_objectToExplore);
            }
            else if (data is PropertyInfo propertyInfo)
            {
                control.UserData = propertyInfo.GetValue(_objectToExplore);
            }
            else
            {
                _logger.Error("UserData should be field/property");
            }
            
            base.HandleArrayClick(control, args);
            control.UserData = data;
        }
        
        private void AddProperties()
        {
            // Properties
            try
            {
                _propertiesProps = CreateGroup("Properties");

                foreach (var property in Properties)
                {
                    if (property.CanRead)
                    {
                        var value = property.GetValue(_objectToExplore);
                        var row = CreateRowForObject(property.Name, value, _propertiesProps);
                        row.UserData = property;
                        row.ValueChanged += HandlePropertyChange;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private void AddMethods()
        {
            var button = _tabs.AddPage("Methods");
            var page = button.Page;

            var listBox = new ListBox(page) {Dock = Pos.Fill};

            foreach (var method in Type.GetMethods(Flags))
            {
                var row = listBox.AddRow($"[{listBox.RowCount}] {method.Name}");
                var payload = (o: _objectToExplore, method);

                row.UserData = payload;
                row.Clicked += HandleOpenMethodInvoker;
            }
        }

        private void HandleFieldChange(Base control, EventArgs args)
        {
            var row = control as PropertyRow;
            var userData = row?.UserData;

            if (userData is FieldInfo field)
            {
                _fieldValues[field] = row.Value;    
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void HandlePropertyChange(Base control, EventArgs args)
        {
            var row = control as PropertyRow;
            var userData = row?.UserData;

            if (userData is PropertyInfo property)
            {
                _propValues[property] = row.Value;    
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}