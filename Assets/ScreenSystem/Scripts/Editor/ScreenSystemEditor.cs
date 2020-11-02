﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Screen = ScreenSystem.Scripts.Control.Screen;

namespace ScreenSystem.Scripts.Editor
{
    [CustomEditor(typeof(Control.ScreenSystem))]
    public class ScreenSystemEditor : UnityEditor.Editor
    {
        private MonoScript _toCreateScreen;

        public override void OnInspectorGUI()
        {
            var system = (Control.ScreenSystem) target;

            #region Create new screen

            _toCreateScreen =
                EditorGUILayout.ObjectField("Create screen", _toCreateScreen, typeof(MonoScript), true) as MonoScript;

            if (_toCreateScreen != null)
            {
                var toCreateClass = _toCreateScreen.GetClass();

                if (!toCreateClass.IsSubclassOf(typeof(Screen)) ||
                    system.AllScreens.Any(x => x.GetType() == toCreateClass))
                {
                    _toCreateScreen = null;

                    return;
                }


                var newScreen = new GameObject();

                newScreen.transform.SetParent(system.transform);

                newScreen.AddComponent(toCreateClass);

                var toSetName = toCreateClass.Name;
                
                var indexesSet = new List<int>();
                
                for (var i = 0; i < toSetName.Length; i++)
                {
                    var gotChar = toSetName[i];

                    if (char.IsUpper(gotChar) && i != 0)
                    {
                        indexesSet.Add(i);
                    }
                }

                var toAddIndex = 0;
                
                indexesSet.ForEach(x =>
                {
                    toSetName = toSetName.Insert(x + toAddIndex, " ");

                    toAddIndex++;
                });
                
                newScreen.name = toSetName;

                _toCreateScreen = null;
            }

            #endregion

            #region Screens

            EditorGUILayout.BeginVertical("box");

            SpaceLabel("All screens");

            foreach (var screen in system.AllScreens)
            {
                if (screen == null)
                    continue;

                EditorGUILayout.BeginHorizontal("");

                GUILayout.Label(screen.name);

                GUILayout.FlexibleSpace();

                var isActive = screen.IsActive;

                isActive = EditorGUILayout.Toggle(isActive);

                screen.gameObject.SetActive(isActive);

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Hide all"))
            {
                system.AllScreens.ToList().ForEach(x => x.gameObject.SetActive(false));
            }

            EditorGUILayout.EndVertical();

            #endregion

            #region Sounds

            EditorGUILayout.BeginVertical("box");

            SpaceLabel("Sounds");

            system.DefaultHideClip =
                EditorGUILayout.ObjectField("Default hide", system.DefaultHideClip, typeof(AudioClip), true) as
                    AudioClip;

            system.DefaultShowClip =
                EditorGUILayout.ObjectField("Default show", system.DefaultShowClip, typeof(AudioClip), true) as
                    AudioClip;

            EditorGUILayout.EndVertical();

            #endregion
        }

        public static void SpaceLabel(string label, int space = 5)
        {
            GUILayout.Label(label);

            GUILayout.Space(space);
        }
    }
}