using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GameModel;
using Gwen;
using Gwen.Control;

namespace GameUI
{
    public class DebugCollectionWindow : Explorer
    {
        private IEnumerable collection;
        private Logger logger;
        private ListBox listBox;
        
        public DebugCollectionWindow(Base parent, IEnumerable collection, Logger logger) : base(parent)
        {
            this.collection = collection;
            this.logger = logger;

            listBox = new ListBox(this) {Dock = Pos.Fill};
            
            Title = $"Array {collection.GetType().Name}";
            
            SetSize(400, 400);
            CreateCollection();
        }

        private void CreateCollection()
        {
            foreach (var obj in collection)
            {
                ListBoxRow row;
                if (obj != null)
                {
                    row = listBox.AddRow($"[{listBox.RowCount}] {obj.GetType().Name}", obj.ToString(), obj);

                    if (!(obj is string || obj is int || obj is float || obj is bool))
                    {
                        if (obj is IEnumerable childCollection)
                        {
                            row.UserData = childCollection;
                            row.Clicked += HandleArrayClick;
                        }
                        else
                        {
                            row.UserData = obj;
                            row.Clicked += HandleClassClick;
                        }
                    }
                }
                else
                {
                    row = listBox.AddRow($"[{listBox.RowCount}]", "null");
                }
                
//                row.Dock = Pos.Fill;
            }
        }
    }
}