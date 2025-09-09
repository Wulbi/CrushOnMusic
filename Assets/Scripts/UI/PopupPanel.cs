using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupPanel : BasePanel
{
    public Button buttonClose;
    public TMP_Text labelName;
    public TMP_Text labelDesc;
    public RectTransform panelRoot;
    
    private string nameMessage;
    private string descMessage;

    private Sequence seq;
    
    public override UIPanelType TypeOfPanel => UIPanelType.POPUP_PANEL;
    private void Awake()
    {
        buttonClose.onClick.AddListener(OnClickClose);
    }

    public override void OnOpenEffect()
    {
        seq?.Kill();
        seq = DOTween.Sequence();
        panelRoot.transform.localScale = Vector3.one * 0.001f;
        seq.Append(panelRoot.DOScale(1f, .25f).SetEase(Ease.OutCubic));
    }

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
 
        nameMessage = "null";
        descMessage = "null";
        
        if (datas.Length > 0)
            nameMessage = (string)datas[0];

        if (datas.Length > 1)
            descMessage = (string)datas[1];
        
        labelName.text = nameMessage;
        labelDesc.text = descMessage;
        
        this.gameObject.SetActive(true);
    }
    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
    }
    public void OnClickClose()
    {
        //팝업 패널 닫기.
        OnClose();
    }
}
