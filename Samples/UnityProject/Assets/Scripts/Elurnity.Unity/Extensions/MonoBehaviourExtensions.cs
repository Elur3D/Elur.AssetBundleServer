using UnityEngine;
using System;
using System.Linq;

namespace Elurnity
{
    public static class MonoBehaviourExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var comp = gameObject.GetComponent<T>();
            if (comp == null)
            {
                comp = gameObject.AddComponent<T>();
            }
            return comp;
        }

        public static T[] GetInterfaces<T>(this Component component) where T : class
        {
            return component.GetComponents(typeof(T)).Cast<T>().ToArray();
        }
    }
}