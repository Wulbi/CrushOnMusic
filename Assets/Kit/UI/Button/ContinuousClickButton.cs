using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameLogic.Core.Optimization;
using DG.Tweening;
using GameLogic.Manager;

/// <summary>
/// 계속 눌러서 호출하는 버튼
/// </summary>
public class ContinuousClickButton : CustomButton, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerClickHandler, IPointerExitHandler, IUpdatable
{
    private const float LongPressEventStartDuration     = 1f;
    private const float LongPressStartDuration          = 0.4f;
    private const float MinDelayBetweenClicks           = 0.1f;

    [SerializeField] private Button             button;
    [SerializeField] protected bool             isContinuousButtonSound     = false;
    [SerializeField] protected bool             isSlient                    = false;
    [SerializeField] private bool               animated;
    
    [SerializeField] protected Image         buttonImage;
    [SerializeField] protected RectTransform lockIcon;
    [SerializeField] protected RectTransform notiIcon;

    [SerializeField] private float            buttonPressedScale = 0.95f;
    [SerializeField] private Ease             buttonPressedEase = Ease.Linear;
    [SerializeField] private float            buttonPressedDuration = 0.08f;
    [SerializeField] private RectTransform    scaleRect;

    public Action OnLongPressEventStarted = null;
    
    public bool IsLock { get; private set; } = false;
    public bool IsNoti { get; private set; } = false;
    
    public Image ButtonImage => buttonImage;
    
    protected override bool IsSlient => isSlient;

    private bool                isPointerDown;
    private bool                isLongPress;
    private double              pointerDownTime;
    private double              lastClickTime;

    protected override Button originButton => button;
    private Tween scaleTweener = null;

    protected override void OnEnable()
    {
        CustomUpdateManager.Add(this);

        if (originButton == null)
            return;
        
        originButton.onClick.AddListener(() => {
            if (!CustomButton.IsClickingAllowed())
                return;

            if (isLongPress)
                return;

            if (!IsSlient)
                SoundManager.Instance.PlaySfx(CommonSounds.GetClip(ClickSoundType));

            this.OnClick.Fire();
        });

        originButton.interactable = this.Interactable;

        if (button != null)
            button.transform.localScale = Vector3.one;

        if (scaleRect != null)
            scaleRect.transform.localScale = Vector3.one;
    }
    protected override void OnDisable()
    {
        CustomUpdateManager.Remove(this);
        
        if (originButton == null)
            return;
        
        originButton.onClick.RemoveAllListeners();

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
    void IUpdatable.OnUpdate()
    {
        
        if (!this.isPointerDown || this.OnClick == null)
            return;

        double num  = TimeProvider.Instance.UnscaledTimeSinceStartup - this.pointerDownTime;

        CheckLongPressEventStarted((float)num);
        
        if (this.isLongPress)
        {
            double num2 = TimeProvider.Instance.UnscaledTimeSinceStartup - this.lastClickTime;
            float num3 = MathUtils.MapClamped((float)num, 0.0f, 1f, LongPressStartDuration, MinDelayBetweenClicks);
            if (!this.Interactable || num2 <= (double)num3)
                return;

            if (isContinuousButtonSound && !IsSlient)
                SoundManager.Instance.PlaySfx(CommonSounds.GetClip(ClickSoundType));


            this.OnClick.Fire();
            this.lastClickTime = TimeProvider.Instance.UnscaledTimeSinceStartup;
        }
        else
        {
            if (num <= LongPressStartDuration)
                return;

            this.isLongPress = true;
        }
    }

    public void CheckLongPressEventStarted(float _num)
    {
        if (_num <= LongPressEventStartDuration)
            return;
        
        //Debug.Log($"num : {_num} {_num <= LongPressEventStartDuration} {OnLongPressEventStarted}");
        OnLongPressEventStarted.Fire();
    }
    public void OnPointerDown(PointerEventData eventData)
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
        this.isPointerDown              = true;
        this.isLongPress                = false;
        this.pointerDownTime            = TimeProvider.Instance.UnscaledTimeSinceStartup;
    }
    public void OnPointerUp(PointerEventData eventData)
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
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!this.isPointerDown)
            return;

        this.isPointerDown = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        this.isPointerDown = false;
        
        
        if (!this.isLongPress)
            return;

        this.isLongPress = false;
    }
    public virtual void SetLock(bool isLock)
    {
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(isLock);

        IsLock = isLock;
    }
    public virtual void SetNoti(bool isNoti)
    {
        if (notiIcon != null)
            notiIcon.gameObject.SetActive(isNoti);

        IsNoti = isNoti;
    }
}
