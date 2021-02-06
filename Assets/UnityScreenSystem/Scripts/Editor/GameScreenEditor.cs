using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenSystem.Scripts.Control;

namespace UnityScreenSystem.Scripts.Editor
{
    [CustomEditor(typeof(GameScreen), true)]
    public class GameScreenEditor : UnityEditor.Editor
    {
        private Sprite _toCreateExample;

        private const string NameExample = "EXAMPLE";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetScreen = (GameScreen) target;
            
            if (targetScreen.transform.childCount <= 0)
                return;

            var firstChild = targetScreen.transform.GetChild(0);

            if (firstChild.name == NameExample)
            {
                firstChild.gameObject.SetActive(EditorGUILayout.Toggle("Is example active: ",
                    firstChild.gameObject.activeSelf));
                
                return;
            }

            _toCreateExample =
                EditorGUILayout.ObjectField("Create example image", _toCreateExample, typeof(Sprite), true,
                    GUILayout.Height(18)) as Sprite;


            if (_toCreateExample != null)
            {
                var newImage = new GameObject()
                {
                    name = NameExample
                };

                var newImageComponent = newImage.AddComponent<Image>();

                var newImageRectTransform = newImage.GetComponent<RectTransform>();

                var newImageTransform = newImage.transform;

                var colorSetUp = Color.white;

                newImageTransform.parent = ((GameScreen) target).transform;

                newImageTransform.SetSiblingIndex(0);

                colorSetUp.a = .4f;

                newImageComponent.sprite = _toCreateExample;

                newImageComponent.color = colorSetUp;

                newImageRectTransform.anchorMin = Vector2.zero;

                newImageRectTransform.anchorMax = Vector2.one;

                newImageRectTransform.offsetMax = Vector2.zero;

                newImageRectTransform.offsetMin = Vector2.zero;

                _toCreateExample = null;
            }
        }
    }
}