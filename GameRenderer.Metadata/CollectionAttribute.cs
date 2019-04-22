using System;
using System.Collections.Generic;

namespace GameRenderer.Metadata
{
    public class CollectionAttribute : Attribute
    {
        private static Dictionary<string, object> collections;
        
        static CollectionAttribute()
        {
            collections = new Dictionary<string, object>();
        }

        private string collectionName;

        public CollectionAttribute(string name)
        {
            collectionName = name;
        }

        static void AddClass(string name, Object t)
        {
            collections.Add(name, t);
        }

        public object GetCollections()
        {
            collections.TryGetValue(collectionName, out var obj);
            return obj;
        }
    }
}