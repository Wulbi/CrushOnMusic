using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SettingPanel : BasePanel
{
    public SimpleButton buttonReset;
    public SimpleButton buttonClose;
    public RectTransform panelRoot;
    
    public override UIPanelType TypeOfPanel => UIPanelType.POPUP_PANEL;
    private void Awake()
    {
        buttonReset.OnClick = OnClickedReset;
        buttonClose.OnClick = OnClickedClose;
    }

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        
        this.gameObject.SetActive(true);
    }
    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }
    public void OnClickedClose()
    {
        //팝업 패널 닫기.
        OnClose();
    }

    private void OnClickedReset()
    {
        DOTween.KillAll(); 
        
        //데이터를 초기화.
        UserSettings.DeleteAll();
        
        //씬을 한번 다시 불러올거야
        SceneManager.LoadScene(0);
    }
}