using System;

namespace GameUtils
{
    public static class ReflectionHelper
    {
        public static T CreateObjectFromType<T>(string typeName)  where T : class
        {
            try
            {
                var type = Type.GetType(typeName);
                if (Activator.CreateInstance(type) is T obj)
                {
                    return obj;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}