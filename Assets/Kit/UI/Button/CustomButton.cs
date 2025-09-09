using System;
using UnityEngine;
using UnityEngine.UI;
using GameLogic.Enum;
using GameLogic.Manager;

public abstract class CustomButton : MonoBehaviour
{
    private static int              nextAllowedClickFrame;
    protected bool                  Interactable = true;
    private RectTransform           rectTransform;
    public Action                   OnClick;
  
    protected abstract Button originButton { get; }
    protected virtual SfxType ClickSoundType        => SfxType.COMMON_BUTTON;
    protected virtual bool IsSlient => false;
    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = this.GetComponent<RectTransform>();
            return rectTransform;
        }
    }
    public bool IsInteractable => Interactable;

    public static bool IsClickingAllowed()
    {
        int frameCount = Time.frameCount;
        if (frameCount < CustomButton.nextAllowedClickFrame)
            return false;

        CustomButton.nextAllowedClickFrame = frameCount + 2;
        return true;
    }

    protected virtual void OnEnable()
    {
        if (originButton == null)
            return;

        originButton.onClick.AddListener(() => {
            if (!CustomButton.IsClickingAllowed())
                return;

            if (!IsSlient)
                SoundManager.Instance.PlaySfx(CommonSounds.GetClip(ClickSoundType));

            this.OnClick.Fire();
        });

        originButton.interactable = this.Interactable;
    }
    protected virtual void OnDisable()
    {
        if (originButton == null)
            return;

        originButton.onClick.RemoveAllListeners();
    }

    public virtual void SetInteractable(bool interactable)
    {
        this.Interactable = interactable;

        if (originButton == null)
            return;
        
        originButton.interactable = interactable;
    }
    public void SetActive(bool active) => gameObject.SetActive(active);
}