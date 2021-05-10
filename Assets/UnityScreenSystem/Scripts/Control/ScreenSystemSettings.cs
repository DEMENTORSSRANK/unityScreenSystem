using UnityEngine;

namespace UnityScreenSystem.Scripts.Control
{
    public class ScreenSystemSettings : ScriptableObject
    {
        [SerializeField] private bool _isDebug = true;

        public bool IsDebug => _isDebug;
    }
}