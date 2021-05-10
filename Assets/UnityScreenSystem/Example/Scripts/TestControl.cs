using System;
using UnityEngine;
using UnityScreenSystem.Example.Scripts.Screens;
using UnityScreenSystem.Scripts.Control;

namespace UnityScreenSystem.Example.Scripts
{
    public class TestControl : MonoBehaviour
    {
        private void Start()
        {
            ScreenSystem.Instance.ShowScreen<SecondTestGameScreen>();
        }
    }
}
