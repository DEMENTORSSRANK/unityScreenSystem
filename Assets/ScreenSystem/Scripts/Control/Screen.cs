using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ScreenSystem.Scripts.Control
{
    public abstract class Screen : MonoBehaviour
    {
        [Header("General")]
        
        [SerializeField] private ScreenType screenType;

        [SerializeField] private bool isShowOnStart;

        [SerializeField] private Screen[] hideWhenShow;

        [Header("Audio")]
        
        [SerializeField] private AudioClip onShowClip;

        [SerializeField] private AudioClip onHideClip;

        [Header("Events")]
        
        [SerializeField] private UnityEvent<Screen> onShowEvent;

        [SerializeField] private UnityEvent<Screen> onHideEvent;

        public UnityEvent<Screen> OnShowEvent
        {
            get => onShowEvent;
            set => onShowEvent = value;
        }

        public UnityEvent<Screen> OnHideEvent
        {
            get => onHideEvent;
            set => onHideEvent = value;
        }

        public UnityAction<Screen> OnShowed { get; set; }

        public UnityAction<Screen> OnHidden { get; set; }

        public bool IsShowOnStart => isShowOnStart;
        
        public bool IsActive => gameObject.activeSelf;

        public ScreenType ScreenType => screenType;

        public AudioClip OnShowClip => onShowClip;

        public AudioClip OnHideClip => onHideClip;

        private ScreenSystem ParentSystem { get; set; }
        
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

        protected virtual void OnShow()
        {
            
        }

        protected virtual void OnHide()
        {
            
        }
    }
}
