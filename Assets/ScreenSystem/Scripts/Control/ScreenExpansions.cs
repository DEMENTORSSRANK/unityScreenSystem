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

        public static Transform[] GetAllChildren(this GameObject[] pool)
        {
            var returnList = new List<Transform>();

            returnList.AddRange(pool.Select(x => x.transform));
            
            foreach (var p in pool)
            {
                returnList.AddRange(p.transform.GetAllChildren());
            }

            return returnList.ToArray();
        }
    }
    
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
		
        public static T Instance 
        { 
            get 
            { 
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
				
				
                return _instance;	
            }
			
            set => _instance = value;
        }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}