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
            ParentSystem = system;
            
            transform.parent = system.transform;
        }

        public void Show(bool hideOthers = false)
        {
            gameObject.SetActive(true);
            
            OnShowed?.Invoke(this);
            
            OnShowEvent?.Invoke(this);

            OnShow();
            
            if (hideOthers)
                hideWhenShow.ToList().ForEach(x => x.Hide());
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            gameObject.SetActive(false);
            
            OnHidden?.Invoke(this);
            
            OnHideEvent?.Invoke(this);

            OnHide();
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
