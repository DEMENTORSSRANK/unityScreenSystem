using UnityScreenSystem.Scripts.Control;

namespace UnityScreenSystem.Example.Scripts.Screens
{
    public class TestGameScreen : GameScreen
    {
        private void Start()
        {
            var screen = ScreenSystem.Instance.FindScreen<TestGameScreen>();
        }
    }
}