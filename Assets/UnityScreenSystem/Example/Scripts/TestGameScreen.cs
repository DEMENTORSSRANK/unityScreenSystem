using System;
using UnityEngine;
using UnityScreenSystem.Scripts.Control;

namespace UnityScreenSystem.Example.Scripts
{
    public class TestGameScreen : GameScreen
    {
        private void Start()
        {
            var screen = ScreenSystem.Instance.FindScreen<TestGameScreen>();
        }
    }
}