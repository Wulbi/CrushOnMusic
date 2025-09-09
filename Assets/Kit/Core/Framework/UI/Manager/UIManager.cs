using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 패널들을 관리하는 클래스
/// </summary>
public class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }

    private UIManager()
    {
        ParseUIPanelTypeJson();
    }

    private Transform canvasTransform;
    public Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                Transform canvasTrans = GameObject.Find("UI Canvas").transform;
                if (canvasTrans == null)
                {
                    Debug.LogWarning("<b>[UIManager]</b> Canvase 이름을 설정이 필요합니다 [Canvas -> UI Canvas]");
                    return null;
                }

                int childCnt = canvasTrans.childCount;
                if (childCnt > 0)
                    canvasTransform = canvasTrans.GetChild(0).GetChild(0).GetChild(0);
                else
                    canvasTransform = canvasTrans;
            }
            return canvasTransform;
        }
    }

    private Dictionary<UIPanelType, string>         panelPathDict;
    private Dictionary<UIPanelType, BasePanel>      panelDict;
    private Stack<BasePanel>                        panelStack;
    
    /// <summary>
    /// 패널을 여는 메소드
    /// </summary>
    /// <param name="panelType">어떤 타입의 패널 오픈?</param>
    /// <param name="_datas">특정 값을 전달하고 싶을 때 사용</param>
    public void PushPanel(UIPanelType panelType, params object[] _datas)
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }
        
        BasePanel panel     = GetPanel(panelType);
        
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek(); 
            if (panel.TypeOfPanelStyle != UIPanelStyleType.WINDOW)
                topPanel.OnPause(); 
                
        }
        
        panel.transform.SetAsLastSibling();
        panel.OnEnter(_datas); 
        panelStack.Push(panel);
    }

    /// <summary>
    /// 패널을 닫거나 없앨 때 사용하는 메소드
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }

        if (panelStack.Count <= 0) return;

        BasePanel topPanel = panelStack.Pop(); 
        topPanel.OnExit();

        if (panelStack.Count <= 0) return;

        BasePanel topPanel2 = panelStack.Peek(); 
        if (topPanel.TypeOfPanelStyle != UIPanelStyleType.WINDOW)
            topPanel2.OnResume(); 
    }

    /// <summary>
    /// 특정 패널에 대한 오브젝트 정보를 가져오기 위한 메소드
    /// </summary>
    /// <param name="panelType">어떤 타입의 패널</param>
    /// <returns></returns>
    public BasePanel GetPanel(UIPanelType panelType)
    {
        if(panelDict == null)
        {
            panelDict = new Dictionary<UIPanelType, BasePanel>();
        }

        BasePanel panel = panelDict.TryGet(panelType);

        if (panel == null) 
        {
            string path = panelPathDict.TryGet(panelType); 
            GameObject insPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            insPanel.transform.SetParent(CanvasTransform,false); 

            if (panelDict.ContainsKey(panelType))
            {
                panelDict[panelType] = insPanel.GetComponent<BasePanel>();
            }
            else
            {
                panelDict.Add(panelType, insPanel.GetComponent<BasePanel>());
            }

            return insPanel.GetComponent<BasePanel>(); 
        }
        else 
        {
            return panel;
        }
    }

    [System.Serializable] 
    class UIPanelTypeJson
    {
        public List<UIPanelInfo> infoList;
    }

    /// <summary>
    /// 최초 리소스 폴더에 있는 UIPanelType 파일을 가지고 패널들을 구성하는 메소드
    /// </summary>
    private void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<UIPanelType, string>(); 

        // 텍스트 에셋 파일을 리소스 폴더에서 읽어오는 코드
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");
        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text); 
 
        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            panelPathDict.Add(info.panelType, info.path);
        }
    }
}
