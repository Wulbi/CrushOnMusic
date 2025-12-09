using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameLogic.Enum;
using GameLogic.Manager;

public class GameRoot : MonoBehaviour
{
    private static bool hasAppStarted = false;
    
    /// <summary>
    /// Reset the app started flag so LogoPanel shows again after reset
    /// </summary>
    public static void ResetAppStarted()
    {
        hasAppStarted = false;
    }
    
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
        // Only open LogoPanel on the first app start, not on scene changes
        if (!hasAppStarted)
        {
            hasAppStarted = true;
            // Ensure MainPanel is closed before opening LogoPanel (important after reset)
            CloseMainPanelIfOpen();
            UIManager.Instance.PushPanel(UIPanelType.LOGO_PANEL);
        }
        else
        {
            // If app has already started and we're loading heliocentrism scene, open MainPanel
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "Heliocentrism")
            {
                SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.MAIN));
                UIManager.Instance.PushPanel(UIPanelType.MAIN_PANEL);
            }
        }
    }
    
    private void CloseMainPanelIfOpen()
    {
        try
        {
            var mainPanel = UIManager.Instance.GetPanel(UIPanelType.MAIN_PANEL);
            if (mainPanel != null && mainPanel.gameObject != null && mainPanel.gameObject.activeSelf)
            {
                mainPanel.gameObject.SetActive(false);
            }
        }
        catch
        {
            // MainPanel might not exist yet, ignore
        }
    }

}
