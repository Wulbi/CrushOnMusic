using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using GameLogic.Enum;

/// <summary>
/// 공통으로 사용하는 기본 버튼 컴포넌트
/// </summary>
public class SimpleButton : CustomButton, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
    [SerializeField] protected Button                   button;
    [SerializeField] protected bool                     isSlient                = false;
    
    [SerializeField] protected bool                     animated;
    [SerializeField] protected TMP_Text                 buttonText;
    [SerializeField] protected Image                    buttonIcon;
    [SerializeField] protected RectTransform            lockIcon;
    [SerializeField] protected RectTransform            notiIcon;

    [SerializeField] protected float                    buttonPressedScale = 0.95f;
    [SerializeField] protected Ease                     buttonPressedEase = Ease.Linear;
    [SerializeField] protected float                    buttonPressedDuration = 0.08f;
    [SerializeField] protected RectTransform            scaleRect;

   
    public bool IsLock { get; private set; } = false;
    public bool IsNoti { get; private set; } = false;
    
    public TMP_Text ButtonText => buttonText;
    public Image ButtonIcon => buttonIcon;
    
    protected override Button originButton => this.button;
    protected override SfxType ClickSoundType => base.ClickSoundType;
    protected override bool IsSlient => isSlient;

    protected Tween scaleTweener = null;

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
