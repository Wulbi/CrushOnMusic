using System;
using GameLogic.Enum;
using GameLogic.Manager;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoPanel : BasePanel
{
    public RectTransform logoRect;
    
    public SimpleButton buttonStart;
    public override UIPanelType TypeOfPanel => UIPanelType.LOGO_PANEL;
    private Sequence seq;
    
    private void StartLogoAnimation()
    {
        seq?.Kill();
        seq = DOTween.Sequence();
        seq.Append(logoRect.DOAnchorPosY(logoRect.anchoredPosition.y + 20f, 1f).SetEase(Ease.InOutSine));
        seq.Append(logoRect.DOAnchorPosY(logoRect.anchoredPosition.y - 20f, 1f).SetEase(Ease.InOutSine));
        seq.SetLoops(-1, LoopType.Yoyo);
    }
    
    
    private void Awake()
    {
        buttonStart.OnClick = OnClickStart;
        //buttonStart.onClick.AddListener(OnClickStart);
    }
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);

        SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.INTRO));
        
        this.gameObject.SetActive(true);

        StartLogoAnimation();
        
        //유저 데이터 로드.
        SetData();
    }

    public void SetData()
    {
        //유저 데이터를 불러온다.
        UserSettings.Init();
        
        GlobalManager.Instance.LoadData();
        
        AchievementManager.Instance.InitData();
        
        EventManager.Instance.TriggerEvent(GameProgressEventType.GAME_STARTED);
    }
    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }
    public void OnClickStart()
    {
        //로고 패널 닫기.
        OnClose();
        //메인 패널 열기.
        SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.MAIN));
        
        UIManager.Instance.PushPanel(UIPanelType.MAIN_PANEL);
    }
}
