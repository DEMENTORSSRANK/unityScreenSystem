﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityScreenSystem.Scripts.Control
{
    public abstract class GameScreen : MonoBehaviour
    {
        [Header("General")] [SerializeField] private bool isShowOnStart = false;

        [SerializeField] private bool changeChildIndex = true;

        [SerializeField] private ScreenType screenType = ScreenType.Window;

        [SerializeField] private GameScreen[] hideWhenShow;

        [Header("Audio")] [SerializeField] private AudioClip onShowClip;

        [SerializeField] private AudioClip onHideClip;

        [Header("Default")] [SerializeField] private Button[] backButtons;

        [Header("Events")] [SerializeField] private UnityEvent<GameScreen> onShowEvent;

        [SerializeField] private UnityEvent<GameScreen> onHideEvent;

        public UnityEvent<GameScreen> OnShowEvent
        {
            get => onShowEvent;
            set => onShowEvent = value;
        }

        public UnityEvent<GameScreen> OnHideEvent
        {
            get => onHideEvent;
            set => onHideEvent = value;
        }

        public UnityAction<GameScreen> OnShowed { get; set; }

        public UnityAction<GameScreen> OnHidden { get; set; }

        public bool IsShowOnStart => isShowOnStart;

        public bool ChangeChildIndex => changeChildIndex;

        public bool IsActive => gameObject.activeSelf;

        public ScreenType ScreenType => screenType;

        public AudioClip OnShowClip => onShowClip;

        public AudioClip OnHideClip => onHideClip;

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

            hideWhenShow.ToList().ForEach(x => x.Hide());

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
            if (backButtons.Length <= 0)
                return;
            
            backButtons.ToList().ForEach(x => x.onClick.AddListener(Hide));
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