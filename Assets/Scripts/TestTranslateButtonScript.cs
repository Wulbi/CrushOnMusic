using SimpleLocalization;
using TMPro;
using UnityEngine;

public class TestTranslateButtonScript : MonoBehaviour
{
    [SerializeField] public SystemLanguage lang;
    [SerializeField] private TMP_Text localizedText;
    private void OnEnable()
    {
        SimpleLocalization.Localizator.OnLanguageChanged += OnLanguageChaged;
    }

    private void OnDisable()
    {
        SimpleLocalization.Localizator.OnLanguageChanged -= OnLanguageChaged;
    }

    public void OnButtonClick()
    {
        Localizator.ChangeLanguage(lang);
        Debug.Log(Localizator.Translate("UI_TEXT_LOGO_TITLE"));
    }
    
    private void OnLanguageChaged()
    {
        localizedText.text = SimpleLocalization.Localizator.Translate("UI_TEXT_LOGO_TITLE");
    }
}
