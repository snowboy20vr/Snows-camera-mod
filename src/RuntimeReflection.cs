using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SnowsCameraMod
{
    internal static class RuntimeReflection
    {
        private static readonly BindingFlags Flags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static object GetMember(object instance, string name)
        {
            if (instance == null) return null;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(name, Flags);
            if (field != null) return field.GetValue(instance);
            PropertyInfo property = type.GetProperty(name, Flags);
            if (property != null && property.CanRead)
            {
                try { return property.GetValue(instance, null); } catch { }
            }
            return null;
        }

        public static object GetStaticMember(Type type, string name)
        {
            if (type == null) return null;
            FieldInfo field = type.GetField(name, Flags);
            if (field != null) return field.GetValue(null);
            PropertyInfo property = type.GetProperty(name, Flags);
            if (property != null && property.CanRead)
            {
                try { return property.GetValue(null, null); } catch { }
            }
            return null;
        }

        public static string GetString(object instance, params string[] names)
        {
            foreach (string name in names)
            {
                object value = GetMember(instance, name);
                if (value is string s && !string.IsNullOrWhiteSpace(s)) return s;
            }
            return null;
        }

        public static Transform FindChild(Transform root, string exactName)
        {
            if (root == null) return null;
            if (root.name == exactName) return root;
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == exactName) return child;
            return null;
        }

        public static Transform FindBestHead(Transform root)
        {
            if (root == null) return null;
            string[] preferred = { "head", "Head", "headTrans", "headTransform", "Main Camera", "Camera", "headCollider" };
            foreach (string name in preferred)
            {
                Transform found = FindChild(root, name);
                if (found != null) return found;
            }
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("head") && !n.Contains("hand")) return t;
            }
            return root;
        }

        public static Transform FindBestBody(Transform root)
        {
            if (root == null) return null;
            Transform body = FindChild(root, "body");
            if (body != null) return body;
            Transform rig = FindChild(root, "rig");
            return rig != null ? rig : root;
        }

        public static bool LooksLikePlayerObject(GameObject go)
        {
            if (go == null) return false;
            string n = go.name.ToLowerInvariant();
            if (n.Contains("vr rig") || n.Contains("vrrig") || n.Contains("gorillaplayer") || n == "player")
                return true;

            foreach (Component component in go.GetComponentsInChildren<Component>(true))
            {
                if (component == null) continue;
                string type = component.GetType().Name.ToLowerInvariant();
                if (type == "vrrig" || type == "gorillaplayer") return true;
            }
            return false;
        }

        public static Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type type = assembly.GetType(fullName, false);
                    if (type != null) return type;
                }
                catch { }
            }
            return null;
        }
    }
}