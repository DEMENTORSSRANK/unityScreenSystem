using System.Collections.Generic;
using System.Linq;
using ScreenSystem.Scripts.Control;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScreenSystem.Scripts.Editor
{
    public class ScreenSystemWindow : EditorWindow
    {
        private static ScreenSystemWindow _window;

        private ScreenOrientation _orientation;

        private Vector2 _defaultView = new Vector2(1920, 1080);

        [MenuItem("Screen System/Show scene compiler")]
        private static void ShowWindow()
        {
            _window = GetWindow<ScreenSystemWindow>();

            _window.titleContent = new GUIContent("Screen System");

            _window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");

            _orientation = (ScreenOrientation) EditorGUILayout.EnumPopup("Orientation", _orientation);

            _defaultView = EditorGUILayout.Vector2Field("Screen size", _defaultView);

            EditorGUILayout.EndVertical();

            var activeScene = SceneManager.GetActiveScene();

            var allObjects = activeScene.GetRootGameObjects();

            if (!GUILayout.Button("Create screen system"))
                return;
            
            var isAnyScreenSystemOnScene =
                allObjects.GetAllChildren().Any(x => x.GetComponent<Control.ScreenSystem>());

            if (isAnyScreenSystemOnScene)
            {
                Debug.Log("Any have");

                return;
            }

            var createdObject = new GameObject("UI");

            var canvas = createdObject.AddComponent<Canvas>();

            var canvasScaler = createdObject.AddComponent<CanvasScaler>();

            var graphicRayCaster = createdObject.AddComponent<GraphicRaycaster>();

            var screenSystem = createdObject.AddComponent<Control.ScreenSystem>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var resX = _defaultView.x;

            var resY = _defaultView.y;

            var rightResolution = _orientation == ScreenOrientation.Landspace
                ? new Vector2(resX, resY)
                : new Vector2(resY, resX);

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            canvasScaler.referenceResolution = rightResolution;

            canvasScaler.matchWidthOrHeight = .5f;

            Selection.activeGameObject = createdObject;
        }

        private enum ScreenOrientation
        {
            Landspace,
            Portrait
        }
    }
}