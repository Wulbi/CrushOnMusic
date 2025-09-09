using System;
using BigNumber;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainContainer : MonoBehaviour
{
    public Image Icon;
    public TMP_Text labelName;
    
    public TMP_Text labelDesc;
    public TMP_Text labelCost;
    public TMP_Text labelLevel;

    public ContinuousClickButton buttonUpgrade;
    public Image buttonImage;

    private void Awake()
    {
        buttonUpgrade.OnClick = OnClickedUpgrade;
    }

    void Start()
    {
        SetData();
    }

    private void Update()
    {
        //버튼 활성화 여부 체크 / 색상 변경
        bool isUpgrade = GlobalManager.Instance.kiwiAmount >= GlobalManager.Instance.GetUpgradeCost();
        //buttonUpgrade.interactable = isUpgrade;
        buttonImage.color = isUpgrade ? Color.green : Color.gray;
    }

    public void SetData()
    {
        Icon.sprite = DatabaseManager.Instance.upgradeDB.mainData.icon;
        labelName.text = DatabaseManager.Instance.upgradeDB.mainData.Name;
        
        labelDesc.text = $"{GlobalManager.Instance.GetTouchAmount()} Likes /touch";
        labelLevel.text = $"Lv.{GlobalManager.Instance.clickLevel}";
        labelCost.text = $"+{GlobalManager.Instance.GetUpgradeCost()}";
    }

    public void OnClickedUpgrade()
    {
        BigDouble upgradeCost = GlobalManager.Instance.GetUpgradeCost();
        if (GlobalManager.Instance.kiwiAmount >= upgradeCost)
        {
            //업그레이드 진행.
            GlobalManager.Instance.kiwiAmount -= upgradeCost;
            GlobalManager.Instance.clickLevel += 1;
            
            AchievementManager.Instance.ChangeAchievement(AchievementType.MAIN_UPGRADE_COUNT, GlobalManager.Instance.clickLevel);
            
            SetData();
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
        }
        
    }
}
