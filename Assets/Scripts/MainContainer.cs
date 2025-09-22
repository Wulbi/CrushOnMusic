using System;
using BigNumber;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainContainer : MonoBehaviour
{
    [Header("UI Refs")]
    public Image Icon;
    public Image IconBackground;
    public TMP_Text labelName;
    public TMP_Text labelDesc;
    public TMP_Text labelCost;
    public TMP_Text labelLevel;

    public ContinuousClickButton buttonUpgrade;  // 커스텀 버튼
    public Image buttonImage;                    // 버튼 배경 이미지

    [Header("Tick")]
    private float nextUiTick;              // UI 업데이트 간격용
    private const float UiTick = 0.15f;    // 매 프레임 갱신 대신 0.15초 주기

    private void Awake()
    {
        if (buttonUpgrade != null)
            buttonUpgrade.OnClick = OnClickedUpgrade;
    }

    private void Start()
    {
        // 텍스트/아이콘 초기 세팅
        SetData();

        // 테마 라벨 색상 일괄 적용
        UIThemeUtil.SetLabel(labelName);
        UIThemeUtil.SetLabel(labelDesc, isSub: true);
        UIThemeUtil.SetLabel(labelCost);
        UIThemeUtil.SetLabel(labelLevel);

        // 시작 시 버튼 비주얼 1회 적용
        ApplyButtonVisuals();
    }

    private void Update()
    {
        // 가벼운 폴링: 0.15초마다만 평가
        if (Time.unscaledTime < nextUiTick) return;
        nextUiTick = Time.unscaledTime + UiTick;

        ApplyButtonVisuals();
    }

    private void ApplyButtonVisuals()
    {
        var gm = GlobalManager.Instance;
        bool affordable = gm.kiwiAmount >= gm.GetUpgradeCost();
        bool canGradeUp = false; // 메인 업그레이드는 등급업 개념이 없다면 항상 false

        // 버튼 색상(성공/불가) 통일 적용
        UIThemeUtil.SetUpgradeButtonVisual(buttonImage, affordable, canGradeUp);

        // 커스텀 버튼에 상호작용 속성이 있다면 같이 반영 (없으면 제거)
        if (buttonUpgrade != null && buttonUpgrade.TryGetComponent<Button>(out var uibtn))
            uibtn.interactable = affordable;
    }

    public void SetData()
    {
        var db = DatabaseManager.Instance.upgradeDB;
        var gm = GlobalManager.Instance;

        if (Icon) Icon.sprite = db.mainData.icon;
        if (labelName)  labelName.text  = db.mainData.Name;
        if (labelDesc)  labelDesc.text  = $"{gm.GetTouchAmount()} Likes /touch";
        if (labelLevel) labelLevel.text = $"Lv.{gm.clickLevel}";
        if (labelCost)  labelCost.text  = $"+{gm.GetUpgradeCost()}";
    }

    public void OnClickedUpgrade()
    {
        var gm = GlobalManager.Instance;
        BigDouble upgradeCost = gm.GetUpgradeCost();

        if (gm.kiwiAmount >= upgradeCost)
        {
            gm.kiwiAmount -= upgradeCost;
            gm.clickLevel += 1;

            AchievementManager.Instance.ChangeAchievement(
                AchievementType.MAIN_UPGRADE_COUNT, gm.clickLevel);

            // 수치가 바뀌었으니 텍스트/코스트 갱신
            SetData();

            // 버튼 비주얼도 즉시 재평가
            ApplyButtonVisuals();
        }
        else
        {
            UIManager.Instance.PushPanel(
                UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
        }
    }
}