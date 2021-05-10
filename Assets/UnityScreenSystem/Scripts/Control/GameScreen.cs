﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityScreenSystem.Scripts.Control
{
    public abstract class GameScreen : MonoBehaviour
    {
        [Header("General")] [SerializeField] private bool _isShowOnStart;

        [SerializeField] private bool _changeChildIndex = true;

        [SerializeField] private ScreenType _screenType = ScreenType.Window;

        [SerializeField] private GameScreen[] _hideWhenShow;

        [Header("Audio")] [SerializeField] private AudioClip _onShowClip;

        [SerializeField] private AudioClip _onHideClip;

        [Header("Default")] [SerializeField] private Button[] _backButtons;

        [Header("Events")] [SerializeField] private UnityEvent<GameScreen> _onShowEvent;

        [SerializeField] private UnityEvent<GameScreen> _onHideEvent;

        public UnityEvent<GameScreen> OnShowEvent
        {
            get => _onShowEvent;
            set => _onShowEvent = value;
        }

        public UnityEvent<GameScreen> OnHideEvent
        {
            get => _onHideEvent;
            set => _onHideEvent = value;
        }

        public UnityAction<GameScreen> OnShowed { get; set; }

        public UnityAction<GameScreen> OnHidden { get; set; }

        public bool IsShowOnStart => _isShowOnStart;

        public bool ChangeChildIndex => _changeChildIndex;

        public bool IsActive => gameObject.activeSelf;

        public ScreenType ScreenType => _screenType;

        public AudioClip OnShowClip => _onShowClip;

        public AudioClip OnHideClip => _onHideClip;

        protected ScreenSystem ParentSystem { get; set; }

        public void SetSystem(ScreenSystem system)
        {
            if (ParentSystem == system)
                return;

            ParentSystem = system;

            transform.SetParent(system.transform);
        }

        public void Show(bool hideOthers = false)
        {
            if (IsActive)
                return;

            if (hideOthers)
                ParentSystem.HideAllScreens();

            OnShow();

            OnShowed?.Invoke(this);

            OnShowEvent?.Invoke(this);

            gameObject.SetActive(true);

            _hideWhenShow.ToList().ForEach(x => x.Hide());

            if (ChangeChildIndex)
                SetToLastChild();
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            if (!IsActive)
                return;

            OnHide();

            OnHidden?.Invoke(this);

            OnHideEvent?.Invoke(this);

            gameObject.SetActive(false);
        }

        [ContextMenu("Show")]
        private void ContextShow()
        {
            Show();
        }

        private void SetToLastChild()
        {
            var parent = transform.parent;

            if (parent == null)
                return;

            transform.SetSiblingIndex(parent.childCount - 1);
        }

        private void InitBackButtons()
        {
            if (_backButtons.Length <= 0)
                return;
            
            _backButtons.ToList().ForEach(x => x.onClick.AddListener(Hide));
        }

        protected virtual void OnEnable()
        {
            InitBackButtons();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
    }
}