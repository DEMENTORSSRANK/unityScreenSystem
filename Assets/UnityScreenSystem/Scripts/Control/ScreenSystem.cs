using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UnityScreenSystem.Scripts.Control
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
        private AudioClip GetScreenShowClip(GameScreen gameScreen)
        {
            if (gameScreen.OnShowClip != null)
                return gameScreen.OnShowClip;

            var screenType = gameScreen.ScreenType;

            return _soundsScreens.Any(x => x.Type == screenType)
                ? _soundsScreens.ToList().Find(x => x.Type == screenType).ShowClip
                : _defaultShowClip;
        }

        private AudioClip GetScreenHideClip(GameScreen gameScreen)
        {
            if (gameScreen.OnHideClip != null)
                return gameScreen.OnHideClip;

            var screenType = gameScreen.ScreenType;

            return _soundsScreens.Any(x => x.Type == screenType)
                ? _soundsScreens.ToList().Find(x => x.Type == screenType).HideClip
                : _defaultHideClip;
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
            OnSomeWindowShow += delegate(GameScreen screen)
            {
                if (_settings.IsDebug)
                    print(screen.name + " shown");
            };

            OnSomeWindowHide += delegate(GameScreen screen)
            {
                if (_settings.IsDebug)
                    print(screen.name + " hidden");
            };
        }

        private void SoundEvent()
        {
            OnSomeWindowHide += delegate(GameScreen screen) { PlaySound(GetScreenShowClip(screen)); };

            OnSomeWindowHide += delegate(GameScreen screen) { PlaySound(GetScreenHideClip(screen)); };
        }
    }

    // Edit some public methods
    public partial class ScreenSystem
    {
        public T FindScreen<T>() where T : GameScreen
        {
            var type = typeof(T);

            if (_allScreens.ToList().All(x => x.GetType() != type))
            {
                throw new Exception($"Screen \"{type.Name}\" cant be found");
            }

            var foundScreen = _allScreens.ToList().Find(x => x.GetType() == type);

            return (T) foundScreen;
        }

        public void HideAllScreens()
        {
            _allScreens.ToList().ForEach(x => x.Hide());
        }

        public T ShowScreen<T>(bool isHideOther = false) where T : GameScreen
        {
            var screen = FindScreen<T>();

            screen.Show();

            return screen;
        }

        public void HideScreen<T>() where T : GameScreen
        {
            FindScreen<T>().Hide();
        }
    }

    [RequireComponent(typeof(ScreenData))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Canvas))]
    // Main control
    public partial class ScreenSystem : Singleton<ScreenSystem>
    {
        [SerializeField] private GameScreen[] _allScreens;

        [SerializeField] private SoundsScreen[] _soundsScreens;

        [SerializeField] private AudioClip _defaultShowClip;

        [SerializeField] private AudioClip _defaultHideClip;

        private AudioSource _source;

        private ScreenSystemSettings _settings;

        private Canvas _canvas;

        public AudioClip DefaultShowClip
        {
            get => _defaultShowClip;
            set => _defaultShowClip = value;
        }

        public AudioClip DefaultHideClip
        {
            get => _defaultHideClip;
            set => _defaultHideClip = value;
        }

        public UnityAction<GameScreen> OnSomeWindowShow { get; set; }

        public UnityAction<GameScreen> OnSomeWindowHide { get; set; }

        public IEnumerable<GameScreen> AllScreens => _allScreens;

        public RenderMode RenderMode => _canvas == null ? GetComponent<Canvas>().renderMode : _canvas.renderMode;

        [ContextMenu("Init all screens")]
        private void InitAllScreens()
        {
            var allChildren = transform.GetAllChildren().Where(x => x.GetComponent<GameScreen>());

            _allScreens = allChildren.ToList().Select(x => x.GetComponent<GameScreen>()).ToArray();

            _allScreens.ToList().ForEach(x => x.SetSystem(this));

            _allScreens.ToList().ForEach(x =>
            {
                x.OnShowEvent?.AddListener(OnSomeWindowShow);

                x.OnHideEvent?.AddListener(OnSomeWindowHide);
            });
        }

        private void CheckStartOpen()
        {
            var toStartOpen = _allScreens.Where(x => x.IsShowOnStart);

            var toHideOther = _allScreens.Where(x => x.IsActive && !x.IsShowOnStart);

            toStartOpen.ToList().ForEach(x => x.Show());

            toHideOther.ToList().ForEach(x => x.Hide());
        }

        private void InitComponents()
        {
            _source = GetComponent<AudioSource>();

            _canvas = GetComponent<Canvas>();
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

            InitComponents();

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