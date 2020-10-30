using System;
using UnityEngine;

namespace ScreenSystem.Scripts.Control
{
    public class ScreenData : MonoBehaviour
    {
        public ScreenSystemSettings ScreenSettings { get; set; }

        private void Awake()
        {
            ScreenSettings = Resources.Load<ScreenSystemSettings>("Settings");
        }
    }
}