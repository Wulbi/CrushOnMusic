using System;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Application.targetFrameRate = 60;
        
#if UNITY_STANDALONE
        Screen.SetResolution(1080, 1920, false);        
#endif
    }

    void Start()
    {
        UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
    }

}
