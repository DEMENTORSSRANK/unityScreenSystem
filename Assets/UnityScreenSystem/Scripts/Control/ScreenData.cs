﻿using System;
using UnityEngine;

namespace UnityScreenSystem.Scripts.Control
{
    public class ScreenData : Singleton<ScreenData>
    {
        public ScreenSystemSettings ScreenSettings { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            ScreenSettings = Resources.Load<ScreenSystemSettings>("Settings");
        }
    }
}