using UnityEngine;

namespace UnityScreenSystem.Scripts.Control
{
    public class ScreenSystemSettings : ScriptableObject
    {
        [SerializeField] private bool isDebug = true;

        public bool IsDebug => isDebug;
    }
}