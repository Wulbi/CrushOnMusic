using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace GameLogic.UI.Custom
{
    public class TabButton : CustomButton, ITabButton, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        [SerializeField] private Button button;
        [SerializeField] protected bool animated;
        [SerializeField] protected float                    buttonPressedScale = 0.95f;
        [SerializeField] protected Ease                     buttonPressedEase = Ease.Linear;
        [SerializeField] protected float                    buttonPressedDuration = 0.08f;
        [SerializeField] protected RectTransform            scaleRect;
        [SerializeField] protected RectTransform            targetOnRect;
        
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite unselectedSprite;

        [SerializeField] private Text buttonText;
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color unselectedTextColor;
        
        [SerializeField] private Image changeSprImg;

        [SerializeField] private Image changeColorImg;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;

        [SerializeField] private GameObject notiObject;
        [SerializeField] private Text notiText;

        [SerializeField] private GameObject lockObject;
        [SerializeField] private Text lockText;

        private Tabs tabs;

        private bool _enabled;
        private bool _selected;
        private bool _notiEnabled;
        private bool _lockEnabled;

        protected Tween scaleTweener = null;
        
        public bool colorEnable = false;
        public bool spriteEnable = false;
        public bool notiEnable = false;
        public bool lockEnable = false;

        public TabType TargetTabType { get; private set; }
        protected override Button originButton => this.button;

        #region Enabled & Selected
        public bool IsEnabled
        {
            set
            {
                this._enabled = value;
            }
        }
        public bool IsSelected
        {
            set 
            {
                this._selected = value;

                if (tabs != null && this._selected)
                    tabs.SwitchTab(TargetTabType);
            }
        }
        #endregion

        #region [Notification]
        public bool IsNotiEnabled
        {
            set
            {
                this._notiEnabled = value;

                if (notiObject != null)
                    notiObject.gameObject.SetActive(_notiEnabled);
            }
        }
        public string NotiText
        {
            set
            {
                if (notiText != null)
                    notiText.text = value;
            }
        }
        #endregion

        #region [Locking]
        public bool IsLockEnabled
        {
            set
            {
                this._lockEnabled = value;
                if (button == null)
                    button = this.GetComponent<Button>();

                button.interactable = this._lockEnabled == false;

                if (lockObject != null)
                    lockObject.SetActive(_lockEnabled);
            }
        }
        public string LockText
        {
            set
            {
                if (lockText != null)
                    lockText.text = value;
            }
        }
        public bool IsLockObjectActive
        {
            set
            {
                this._lockEnabled = value;

                if (lockObject != null)
                    lockObject.SetActive(_lockEnabled);
            }
        }
        public string ButtonText
        {
            set
            {
                if (buttonText != null)
                    buttonText.text = value;
            }
        }
        #endregion
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (button != null)
                button.transform.localScale = Vector3.one;

            if (scaleRect != null)
                scaleRect.transform.localScale = Vector3.one;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if (scaleTweener != null)
            {
                scaleTweener.Kill(false);
                scaleTweener = null;
            }

            if (button != null)
                button.transform.localScale = Vector3.one;

            if (scaleRect != null)
                scaleRect.transform.localScale = Vector3.one;
        }
         public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (button == null)
        {
            Debug.LogError($"_button prefab is null {this.gameObject?.name}");
            return;
        }
        if (!this.button.interactable)
            return;

        if (this.animated)
        {
            if (button != null)
            {
                if (scaleTweener != null)
                {
                    scaleTweener.Kill(false);
                    scaleTweener = null;
                }

                if (scaleRect != null)
                {
                    scaleRect.transform.localScale = Vector3.one;
                    scaleTweener = scaleRect.transform.DOScale(this.buttonPressedScale, buttonPressedDuration).SetEase(buttonPressedEase).OnComplete(() => {
                        scaleRect.transform.localScale = Vector3.one * this.buttonPressedScale;
                        scaleTweener = null;
                    }).SetUpdate(true);
                }
                else
                {
                    button.transform.localScale = Vector3.one;
                    scaleTweener = button.transform.DOScale(this.buttonPressedScale, buttonPressedDuration).SetEase(buttonPressedEase).OnComplete(() => {
                        button.transform.localScale = Vector3.one * this.buttonPressedScale;
                        scaleTweener = null;
                    }).SetUpdate(true);
                }
            }
        }
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (button == null)
        {
            Debug.LogError($"_button prefab is null {this.gameObject?.name}");
            return;
        }
        if (!this.button.interactable)
            return;

        if (this.animated)
        {
            if (scaleTweener != null)
            {
                scaleTweener.Kill(false);
                scaleTweener = null;
            }

            if (scaleRect != null)
            {
                scaleRect.transform.localScale = Vector3.one * this.buttonPressedScale;
                scaleTweener = scaleRect.transform.DOScale(1f, buttonPressedDuration).SetEase(buttonPressedEase).OnComplete(() => {
                    scaleRect.transform.localScale = Vector3.one;
                    scaleTweener = null;
                }).SetUpdate(true);
            }
            else
            {
                button.transform.localScale = Vector3.one * this.buttonPressedScale;
                scaleTweener = button.transform.DOScale(1f, buttonPressedDuration).SetEase(buttonPressedEase).OnComplete(() => {
                    button.transform.localScale = Vector3.one;
                    scaleTweener = null;
                }).SetUpdate(true);
            }
        }
    }
    
        public RectTransform thisRectTransform => this.GetComponent<RectTransform>();
        public void Init(Tabs tabs, TabType targetTab)
        {
            this.tabs = tabs;
            this.TargetTabType = targetTab;

            if (colorEnable)
            {
                if (changeColorImg == null)
                    this.GetComponent<Image>().color = unselectedColor;
                else
                    changeColorImg.color = unselectedColor;
            }
            
            OnClick = () => { this.tabs.SwitchTab(TargetTabType); };
        }    
        public void OnTabSwitched(TabType tabType)
        {
            bool flag = tabType == this.TargetTabType;

            if (targetOnRect != null)
                targetOnRect.gameObject.SetActive(flag);
            
            if (colorEnable)
            {
                if (changeColorImg == null)
                    this.GetComponent<Image>().color = !flag ? unselectedColor : selectedColor;
                else
                    changeColorImg.color = !flag ? unselectedColor : selectedColor;
            }

            if (spriteEnable)
            {
                if (changeSprImg == null)
                    this.GetComponent<Image>().sprite = !flag ? unselectedSprite : selectedSprite;
                else
                    changeSprImg.sprite = !flag ? unselectedSprite : selectedSprite;
            }
        }
    }
}

