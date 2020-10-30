using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScreenSystem.Scripts.Control
{
    public static class ScreenExpansions
    {
        public static IEnumerable<Transform> GetAllChildren(this Transform parent)
        {
            return parent.GetComponentsInChildren<Transform>(parent).Where(x => x != parent);
        }
    }
    
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}