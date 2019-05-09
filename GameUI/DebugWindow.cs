using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class DebugWindow : Explorer
    {
        private object o;

        private Base parent;

        private Color publicColor = Color.Lime;
        private Color privateColor = Color.Red;
        private Color readonlyColor = Color.Maroon;
        private Type type => o.GetType();

        private Properties fieldProps;
        private Properties propertiesProps;

        private GroupBox root;
        private BindingFlags flags => BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        private FieldInfo[] fields => type.GetFields(flags);
        private PropertyInfo[] properties => type.GetProperties(flags);
        
        private Dictionary<PropertyInfo, string> propValues = new Dictionary<PropertyInfo, string>();
        private Dictionary<FieldInfo, string> fieldValues = new Dictionary<FieldInfo, string>();
        
        private Logger logger;

        private TabControl tabs;
        
        public DebugWindow(Base parent, object o, Logger logger) : base(parent)
        {
            this.parent = parent;
            this.o = o;
            this.logger = logger;

            Title = $"Object {o.GetType().Name}";
            logger.Info($"Creating window for object {o}");

            SetSize(400, 400);
            CreateTabs();
            
            var updateButton = new Button(this) { Dock = Pos.Bottom, Text = "Set values"};
            updateButton.Clicked += HandleUpdateClick;
            
            var refreshButton = new Button(this) { Dock = Pos.Bottom, Text = "Update values"};
            refreshButton.Clicked += HandleRefreshClick;
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
            tabs = new TabControl(this) {Dock = Pos.Fill};
            AddFields();
            AddProperties();
            AddMethods();
        }
        
        public void Refresh()
        {
            RemoveChild(tabs, true);
            CreateTabs();
        }
        public void Update()
        {
            UpdateProps();
            UpdateFields();
            Refresh();
        }
        private void UpdateProps()
        {
            try
            {
                foreach (var prop in propValues)
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
                    
                    logger.Info($"Updating property {property.Name} with value {value}");
                    property.SetMethod.Invoke(o, new[] {value});
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            
            propValues.Clear();
        }
        
        private void UpdateFields()
        {
            try
            {
                foreach (var prop in fieldValues)
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

                    logger.Info($"Updating property {field.Name} with value {value}");
                    field.SetValue(o, value);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            
            fieldValues.Clear();
        }

        private Properties CreateGroup(string name, int position)
        {
            var button = tabs.AddPage(name);
            var page = button.Page;
            
            var scroll = new ScrollControl(page);
            scroll.EnableScroll(true, true);
            scroll.Dock = Pos.Fill;
            var props = new Properties(scroll);
            props.Dock = Pos.Fill;
            return props;
        }

        private void AddFields()
        {
            // All fields
           fieldProps = CreateGroup("Fields", 0);

            try
            {
                foreach (var field in fields)
                {
                    var value = field.GetValue(o);

                    var row = createRowForObject(field.Name, value, fieldProps);
                    row.UserData = field;
                    row.ValueChanged += HandleFieldChange;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private PropertyRow createRowForObject(string name, object obj, Properties props)
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
            if (data is FieldInfo finfo)
            {
                control.UserData = finfo.GetValue(o);
            } else if (data is PropertyInfo pinfo)
            {
                control.UserData = pinfo.GetValue(o);
            }
            else
            {
                logger.Error("UserData should be field/property");
            }
            
            base.HandleClassClick(control, args);
            control.UserData = data;
        }
        
        protected override void HandleArrayClick(Base control, EventArgs args)
        {
            var data = control.UserData;
            // Dirty hack
            if (data is FieldInfo finfo)
            {
                control.UserData = finfo.GetValue(o);
            } else if (data is PropertyInfo pinfo)
            {
                control.UserData = pinfo.GetValue(o);
            }
            else
            {
                logger.Error("UserData should be field/property");
            }
            
            base.HandleArrayClick(control, args);
            control.UserData = data;
        }
        
        private void AddProperties()
        {
            // Properties
            try
            {
                propertiesProps = CreateGroup("Properties", 1);

                foreach (var property in properties)
                {
                    if (property.CanRead)
                    {
                        var value = property.GetValue(o);
                        var str = value?.ToString() ?? "null";
                        
                        var row = createRowForObject(property.Name, value, propertiesProps);
                        row.UserData = property;
                        row.ValueChanged += HandlePropertyChange;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private void AddMethods()
        {
            var button = tabs.AddPage("Methods");
            var page = button.Page;
            
            var listBox = new ListBox(page);
            listBox.Dock = Pos.Fill;

            foreach (var method in type.GetMethods(flags))
            {
                var row = listBox.AddRow($"[{listBox.RowCount}] {method.Name}");
                var payload = (o, method);

                row.UserData = payload;
                row.Clicked += HandleOpenMethodInvoker;
            }
        }

        private void HandleFieldChange(Base control, EventArgs args)
        {
            var row = control as PropertyRow;
            var o = row.UserData;

            if (o is FieldInfo field)
            {
                fieldValues[field] = row.Value;    
            }
            else
            {
                throw new ArgumentException();
            }
//            try
//            {
//                var field = type.GetField(row.Label, flags);
//                field.SetValue(o, row.Value);
//            }
//            catch (Exception e)
//            {
//                logger.Error(e.Message);
//                throw e;
//            }
        }

        private void HandlePropertyChange(Base control, EventArgs args)
        {
            var row = control as PropertyRow;
            var o = row.UserData;

            if (o is PropertyInfo property)
            {
                propValues[property] = row.Value;    
            }
            else
            {
                throw new ArgumentException();
            }
            
//            try
//            {
//                var property = type.GetProperty(row.Label, flags);
//                property.SetValue(o, row.Value);
//            }
//            catch (Exception e)
//            {
//                logger.Error(e.Message);
//                throw e;
//            }
        }
    }
}