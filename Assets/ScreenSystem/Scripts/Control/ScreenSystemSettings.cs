﻿using UnityEngine;

namespace ScreenSystem.Scripts.Control
{
    public class ScreenSystemSettings : ScriptableObject
    {
        [SerializeField] private bool isDebug = true;

        public bool IsDebug => isDebug;
    }
}