using System;
using System.Collections.Generic;
using System.Linq;
using ScreenSystem.Scripts.Screens;
using UnityEngine;
using UnityEngine.Events;

namespace ScreenSystem.Scripts.Control
{
    // Sound set up
    public partial class ScreenSystem
    {
        private void PlaySound(AudioClip clip)
        {
            if (clip == null)
                return;
            
            _source.PlayOneShot(clip);
        }
        
        // TODO: Change some getting screen sound
        private AudioClip GetScreenShowClip(Screen screen)
        {
            if (screen.OnShowClip != null)
                return screen.OnShowClip;

            var screenType = screen.ScreenType;

            return soundsScreens.Any(x => x.Type == screenType)
                ? soundsScreens.ToList().Find(x => x.Type == screenType).ShowClip
                : defaultShowClip;
        }

        private AudioClip GetScreenHideClip(Screen screen)
        {
            if (screen.OnHideClip != null)
                return screen.OnHideClip;

            var screenType = screen.ScreenType;

            return soundsScreens.Any(x => x.Type == screenType)
                ? soundsScreens.ToList().Find(x => x.Type == screenType).HideClip
                : defaultHideClip;
        }
        
        [Serializable]
        private struct SoundsScreen
        {
            [SerializeField] private ScreenType type;

            [SerializeField] private AudioClip showClip;

            [SerializeField] private AudioClip hideClip;

            public ScreenType Type => type;

            public AudioClip ShowClip => showClip;

            public AudioClip HideClip => hideClip;
        }
    }
    
    // Events set
    public partial class ScreenSystem
    {
        private void LogEvent()
        {
            OnSomeWindowShow += delegate(Screen screen)
            {
                if (_settings.IsDebug)
                    print(screen.name + " shown");
            };

            OnSomeWindowHide += delegate(Screen screen)
            {
                if (_settings.IsDebug)
                    print(screen.name + " hidden");
            };
        }

        private void SoundEvent()
        {
            OnSomeWindowHide += delegate(Screen screen) { PlaySound(GetScreenShowClip(screen)); };

            OnSomeWindowHide += delegate(Screen screen) { PlaySound(GetScreenHideClip(screen)); };
        }
    }
    
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

    [RequireComponent(typeof(ScreenData))]
    [RequireComponent(typeof(AudioSource))]
    // Main control
    public partial class ScreenSystem : Singleton<ScreenSystem>
    {
        [SerializeField] private Screen[] allScreens;

        [SerializeField] private SoundsScreen[] soundsScreens;

        [SerializeField] private AudioClip defaultShowClip;

        [SerializeField] private AudioClip defaultHideClip;

        private AudioSource _source;

        private ScreenSystemSettings _settings;

        public AudioClip DefaultShowClip
        {
            get => defaultShowClip;
            set => defaultShowClip = value;
        }

        public AudioClip DefaultHideClip
        {
            get => defaultHideClip;
            set => defaultHideClip = value;
        }

        public UnityAction<Screen> OnSomeWindowShow { get; set; }

        public UnityAction<Screen> OnSomeWindowHide { get; set; }

        public IEnumerable<Screen> AllScreens => allScreens;

        [ContextMenu("Init all screens")]
        private void InitAllScreens()
        {
            var allChildren = transform.GetAllChildren().Where(x => x.GetComponent<Screen>());

            allScreens = allChildren.ToList().Select(x => x.GetComponent<Screen>()).ToArray();

            allScreens.ToList().ForEach(x => x.SetSystem(this));

            allScreens.ToList().ForEach(x =>
            {
                x.OnShowEvent?.AddListener(OnSomeWindowShow);

                x.OnHideEvent?.AddListener(OnSomeWindowHide);
            });
        }

        private void CheckStartOpen()
        {
            var toStartOpen = allScreens.Where(x => x.IsShowOnStart);

            var toHideOther = allScreens.Where(x => x.IsActive && !x.IsShowOnStart);
            
            toStartOpen.ToList().ForEach(x => x.Show());
            
            toHideOther.ToList().ForEach(x => x.Hide());
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isEditor)
                return;

            InitAllScreens();
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
            _settings = ScreenData.Instance.ScreenSettings;
            
            CheckStartOpen();
        }
    }
}