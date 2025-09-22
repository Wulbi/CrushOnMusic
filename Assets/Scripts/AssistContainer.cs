using System;
using BigNumber;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AssistContainer : MonoBehaviour
{
    [Header("UI Refs")]
    public Image Icon;
    public Image IconBackground;
    public TMP_Text labelName;
    public TMP_Text labelDesc;
    public TMP_Text labelCost;
    public TMP_Text labelLevel;

    public Button buttonUpgrade;
    public Image buttonImage;

    public Button buttonMute;
    public Image buttonMuteImage;

    public int order;
    public int level = 0;
    public int grade = 0;

    public MainPanel mainPanel;
    public Musician linkedMusician;

    [Header("Tick")]
    private float nextUiTick;              // UI 업데이트 간격용
    private const float UiTick = 0.15f;    // 매 프레임 갱신 대신 0.15초 주기

    public UpgradeDB.AssistUpgradeData Data 
        => DatabaseManager.Instance.upgradeDB.assistDataList[order];

    public bool CanBeUpgradeGrade()
    {
        if (Data.gradeDataList == null) return false;
        if (Data.gradeDataList.Count <= grade) return false;
        return level >= Data.gradeDataList[grade].needLevel && Data.gradeDataList[grade].needLevel > 0;
    }

    void Start()
    {
        // 최초 색/라벨 일괄 적용
        UIThemeUtil.SetLabel(labelName);
        UIThemeUtil.SetLabel(labelDesc, isSub:true);
        UIThemeUtil.SetLabel(labelCost);
        UIThemeUtil.SetLabel(labelLevel);

        SetData();
        ApplyVisuals(force:true);
    }

    void Update()
    {
        if (Time.unscaledTime < nextUiTick) return;
        nextUiTick = Time.unscaledTime + UiTick;

        ApplyVisuals();
    }

    private void ApplyVisuals(bool force = false)
    {
        var gm = GlobalManager.Instance;

        bool affordable = gm.kiwiAmount >= gm.GetAssistUpgradeCost(order, level, grade);
        bool canGradeUp = CanBeUpgradeGrade();

        // 버튼 색상 통일 적용
        UIThemeUtil.SetUpgradeButtonVisual(buttonImage, affordable, canGradeUp);

        // 버튼 상호작용은 색과 별개로 명확히
        if (buttonUpgrade && buttonUpgrade.interactable != affordable)
            buttonUpgrade.interactable = affordable;

        // 레벨 라벨
        var newLevelText = canGradeUp ? "Upgrade" : $"Lv.{level}";
        if (labelLevel.text != newLevelText) labelLevel.text = newLevelText;
    }

    public void SetData()
    {
        Icon.sprite     = Data.icon;
        labelName.text  = Data.Name;
        labelDesc.text  = $"{GlobalManager.Instance.GetAssistAmount(order, level)} Likes /s";
        labelCost.text  = $"+{GlobalManager.Instance.GetAssistUpgradeCost(order, level, grade)}";
    }

    public void OnClickedUpgrade()
    {
        var gm = GlobalManager.Instance;
        BigDouble cost = gm.GetAssistUpgradeCost(order, level, grade);

        if (gm.kiwiAmount < cost)
        {
            UIManager.Instance.PushPanel(UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
            return;
        }

        gm.kiwiAmount -= cost;

        if (CanBeUpgradeGrade())
        {
            grade += 1;
            gm.assistClickLevelList[order].grade = grade;
        }
        else
        {
            level += 1;
            gm.assistClickLevelList[order].level = level;
        }

        SetData();
        mainPanel.SetContainers();
        EventManager.Instance.TriggerEvent(GameProgressEventType.ASSIST_VIEW_UPGRADE, this);

        // 업그레이드 직후 색/문구 최신화
        ApplyVisuals(force:true);
    }

    public void SetMuteState(bool muted)
    {
        linkedMusician?.Mute(muted);
        UIThemeUtil.SetMuteVisual(buttonMuteImage, muted);
    }

    public void OnClickMute()
    {
        bool muteState = !GlobalManager.Instance.assistClickLevelList[order].isMuted;
        GlobalManager.Instance.assistClickLevelList[order].isMuted = muteState;
        SetMuteState(muteState);
    }
}
