using System;
using System.Collections.Generic;
using System.Xml;

namespace GameRenderer.Animation.ColladaParser.Loader
{
    public static class XmlNodeHelper
    {
        public static XmlNode GetChildWithAttribute(this XmlNode node, string name, string attribute, string value)
        {
            if (!node.HasChildNodes)
            {
                return null;
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                var v = childNode.Attributes?[attribute];

                if (v != null && v.Value.Equals(value))
                {
                    return childNode;
                }
            }

            return null;
        }
        
        public static List<XmlNode> GetChildren(this XmlNode node, string name)
        {
            var list = new List<XmlNode>();

            if (!node.HasChildNodes)
            {
                return list;
            }
            
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == name)
                {
                    list.Add(child);
                }
            }
            
            return list;
        }
        
        public static XmlNode GetChild(this XmlNode node, string name) {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }

            return null;
        }

    }
}