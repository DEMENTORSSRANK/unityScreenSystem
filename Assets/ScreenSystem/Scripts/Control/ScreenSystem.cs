using System;
using System.Linq;
using ScreenSystem.Scripts.Screens;
using UnityEngine;
using UnityEngine.Events;

namespace ScreenSystem.Scripts.Control
{
    // Edit some public methods
    public partial class ScreenSystem
    {
        public Screen GetScreen<T>() where T : Screen
        {
            var foundScreen = allScreens.ToList().Find(x => x.GetType() == typeof(T));

            return foundScreen;
        }

        public void HideAllScreens()
        {
            allScreens.ToList().ForEach(x => x.Hide());
        }

        public void ShowScreen<T>(bool isHideOther = false) where T : Screen
        {
            GetScreen<T>().Show(isHideOther);
        }

        public void HideScreen<T>() where T : Screen
        {
            GetScreen<T>().Hide();
        }
    }

    [RequireComponent(typeof(AudioSource))]
    // Main control
    public partial class ScreenSystem : Singleton<ScreenSystem>
    {
        [SerializeField] private Screen[] allScreens;

        [SerializeField] private SoundsScreen[] soundsScreens;

        [SerializeField] private AudioClip defaultOpenClip;

        [SerializeField] private AudioClip defaultHideClip;

        private AudioSource _source;

        public UnityAction<Screen> OnSomeWindowShow { get; set; }

        public UnityAction<Screen> OnSomeWindowHide { get; set; }

        [ContextMenu("Init all screens")]
        private void InitAllScreens()
        {
            var allChildren = transform.GetAllChildren();

            allScreens = allChildren.ToList().Select(x => x.GetComponent<Screen>()).ToArray();

            allScreens.ToList().ForEach(x => x.SetSystem(this));

            allScreens.ToList().ForEach(x =>
            {
                x.OnShowEvent.AddListener(OnSomeWindowShow);

                x.OnHideEvent.AddListener(OnSomeWindowHide);
            });
        }

        private void PlaySound(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }
        
        private void LogEvent()
        {
            OnSomeWindowShow += delegate(Screen screen) { print(screen.name + " shown"); };

            OnSomeWindowHide += delegate(Screen screen) { print(screen.name + " shown"); };
        }

        private void SoundEvent()
        {
            OnSomeWindowHide += delegate(Screen screen)
            {
                PlaySound(GetScreenShowClip(screen));
            };
            
            OnSomeWindowHide += delegate(Screen screen)
            {
                PlaySound(GetScreenHideClip(screen));
            };
        }

        private void CheckStartOpen()
        {
            var toStartOpen = allScreens.Where(x => x.IsShowOnStart);

            toStartOpen.ToList().ForEach(x => x.Show());
        }

        // TODO: Change some getting screen sound
        private AudioClip GetScreenShowClip(Screen screen)
        {
            if (screen.OnShowClip != null)
                return screen.OnShowClip;

            var screenType = screen.ScreenType;

            return soundsScreens.Any(x => x.Type == screenType)
                ? soundsScreens.ToList().Find(x => x.Type == screenType).ShowClip
                : null;
        }
        
        private AudioClip GetScreenHideClip(Screen screen)
        {
            if (screen.OnHideClip != null)
                return screen.OnHideClip;

            var screenType = screen.ScreenType;

            return soundsScreens.Any(x => x.Type == screenType)
                ? soundsScreens.ToList().Find(x => x.Type == screenType).HideClip
                : null;
        }
        
        protected override void Awake()
        {
            base.Awake();

            _source = GetComponent<AudioSource>();

            LogEvent();
            
            SoundEvent();

            InitAllScreens();
        }

        private void Start()
        {
            CheckStartOpen();
        }

        [Serializable]
        public struct SoundsScreen
        {
            [SerializeField] private ScreenType type;

            [SerializeField] private AudioClip showClip;

            [SerializeField] private AudioClip hideClip;

            public ScreenType Type => type;

            public AudioClip ShowClip => showClip;

            public AudioClip HideClip => hideClip;
        }
    }
}