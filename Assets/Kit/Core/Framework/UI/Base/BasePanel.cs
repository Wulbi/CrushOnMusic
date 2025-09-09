using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour {

    /// <summary>
    /// 패널 타입
    /// </summary>
    public virtual UIPanelType TypeOfPanel
    {
        get { return UIPanelType.NONE; }
        private set { }
    }
    public virtual UIPanelStyleType TypeOfPanelStyle
    {
        get { return UIPanelStyleType.PANEL; }
        private set { }
    }
    /// <summary>
    /// 패널이 처음 활성화 될 때
    /// </summary>
    /// <param name="datas">필요한 데이터가 있는 경우</param>
    public virtual void OnEnter(params object[] datas)
    {
        OnOpenEffect();
    }
    public virtual void OnPause() { }
    public virtual void OnResume() { }
    public virtual void OnExit() { }

    /// <summary>
    /// 패널을 비활성화 할 때
    /// </summary>
    public virtual void OnClose()
    {
        UIManager.Instance.PopPanel();
    }
    
    public virtual void OnOpenEffect() { }
    public virtual void OnBackButton() { }
}
