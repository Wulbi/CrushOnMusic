using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SimpleLocalization;
using UnityEngine.SceneManagement;

public class SettingPanel : BasePanel
{
    public SimpleButton buttonReset;
    public SimpleButton buttonClose;
    public RectTransform panelRoot;

    public Button englishButton;
    public Button koreanButton;
    public TMP_Text labelTitle;
    
    public override UIPanelType TypeOfPanel => UIPanelType.POPUP_PANEL;
    private void Awake()
    {
        buttonReset.OnClick = OnClickedReset;
        buttonClose.OnClick = OnClickedClose;
        
        englishButton.onClick.AddListener(OnClickedEnglish);
        koreanButton.onClick.AddListener(OnClickedKorean);
        
        labelTitle.text = Localizator.Translate("UI_TEXT_LOGO_TITLE");
    }

    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        this.gameObject.SetActive(true);
        SimpleLocalization.Localizator.OnLanguageChanged += OnLanguageChaged;
    }

    private void OnLanguageChaged()
    {
        labelTitle.text = SimpleLocalization.Localizator.Translate("UI_TEXT_LOGO_TITLE");
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActive(false);
        SimpleLocalization.Localizator.OnLanguageChanged -= OnLanguageChaged;
    }
    public void OnClickedClose()
    {
        //팝업 패널 닫기.
        OnClose();
    }

    private void OnClickedReset()
    {
        StartCoroutine(ResetCoroutine());
    }
    
    private IEnumerator ResetCoroutine()
    {
        DOTween.KillAll();
        yield return null; 

        // Reset managers' data before deleting user settings
        if (GlobalManager.HasInstance)
        {
            GlobalManager.Instance.ResetData();
        }
        
        if (AchievementManager.HasInstance)
        {
            AchievementManager.Instance.ResetData();
        }
        
        yield return null;

        UserSettings.DeleteAll();
        yield return null; 

        GameRoot.ResetAppStarted();
        yield return null; 

        // Close all panels before reloading scene
        CloseAllPanels();
        yield return null;

        SceneManager.LoadScene(0);
        
    }
    
    private void CloseAllPanels()
    {
        // Close MainPanel explicitly if it exists and is active
        try
        {
            var mainPanel = UIManager.Instance.GetPanel(UIPanelType.MAIN_PANEL);
            if (mainPanel != null && mainPanel.gameObject != null)
            {
                mainPanel.OnClose();
            }
        }
        catch
        {
            // Panel might not exist or already destroyed, ignore
        }
        
        // Pop all panels from the stack until empty
        // PopPanel returns early if stack is empty, so we'll pop multiple times to clear
        for (int i = 0; i < 10; i++) // Max 10 panels should be enough
        {
            UIManager.Instance.PopPanel();
        }
    }

    private void OnClickedEnglish()
    {
        Localizator.ChangeLanguage(SystemLanguage.English);
    }

    private void OnClickedKorean()
    {
        Localizator.ChangeLanguage(SystemLanguage.Korean);
    }
}