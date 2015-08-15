using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace netLogic.Utilities
{
    public static class Reflection
    {
        public static T GetField<T>(object obj, string name)
        {
            FieldInfo fi = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null)
                throw new KeyNotFoundException("The requested field was not found in the type!");

            return (T)fi.GetValue(obj);
        }

        public static T GetProperty<T>(object obj, string name)
        {
            PropertyInfo fi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null)
                throw new KeyNotFoundException("The requested property was not found in the type!");

            return (T)fi.GetValue(obj, null);
        }

        public static void CallMethod(object obj, string methodName, params object[] args)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
                throw new KeyNotFoundException("The requested method was not found in the type!");

            method.Invoke(obj, args);
        }
    }
}
