﻿using System;
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

            return soundsScreens.Any(x => x.Type == screenType)
                ? soundsScreens.ToList().Find(x => x.Type == screenType).ShowClip
                : defaultShowClip;
        }

        private AudioClip GetScreenHideClip(GameScreen gameScreen)
        {
            if (gameScreen.OnHideClip != null)
                return gameScreen.OnHideClip;

            var screenType = gameScreen.ScreenType;

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
            var foundScreen = allScreens.ToList().Find(x => x.GetType() == typeof(T));

            return (T) foundScreen;
        }

        public void HideAllScreens()
        {
            allScreens.ToList().ForEach(x => x.Hide());
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
    // Main control
    public partial class ScreenSystem : Singleton<ScreenSystem>
    {
        [SerializeField] private GameScreen[] allScreens;

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

        public UnityAction<GameScreen> OnSomeWindowShow { get; set; }

        public UnityAction<GameScreen> OnSomeWindowHide { get; set; }

        public IEnumerable<GameScreen> AllScreens => allScreens;

        [ContextMenu("Init all screens")]
        private void InitAllScreens()
        {
            var allChildren = transform.GetAllChildren().Where(x => x.GetComponent<GameScreen>());

            allScreens = allChildren.ToList().Select(x => x.GetComponent<GameScreen>()).ToArray();

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