using System;
using BigNumber;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AssistContainer : MonoBehaviour
{
    public Image Icon;
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
    
    private float lastTimeUpdate = 0f;
    
    public UpgradeDB.AssistUpgradeData Data => DatabaseManager.Instance.upgradeDB.assistDataList[order];
    
    /// <summary>
    /// 승급 체크
    /// </summary>
    /// <returns>승급 여부</returns>
    public bool CanBeUpgradeGrade()
    {
        //승급 데이터 리스트가 없는 경우
        if (Data.gradeDataList == null)
            return false;
        //현재 등급이 최대일 때 (더 이상 승급을 하지 못하는 경우)
        if (Data.gradeDataList.Count <= grade)
            return false;

        return level >= Data.gradeDataList[grade].needLevel && Data.gradeDataList[grade].needLevel > 0;
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetData();
    }

    // Update is called once per frame
    void Update()
    {
        //버튼 활성화 여부 체크 / 색상 변경
        bool isUpgrade = GlobalManager.Instance.kiwiAmount >= GlobalManager.Instance.GetAssistUpgradeCost(order, level, grade);
        //buttonUpgrade.interactable = isUpgrade;
        buttonImage.color = isUpgrade ? (CanBeUpgradeGrade() ? Color.yellow : Color.green): Color.gray;
        
    }
    
    public void SetData()
    {
        Icon.sprite     = Data.icon;
        labelName.text  = Data.Name;
        labelDesc.text  = $"{GlobalManager.Instance.GetAssistAmount(order, level)} Likes /s";
        labelLevel.text = CanBeUpgradeGrade() ? "Upgrade" : $"Lv.{level}";
        labelCost.text  = $"+{GlobalManager.Instance.GetAssistUpgradeCost(order, level, grade)}";
    }

    
    public void OnClickedUpgrade()
    {
        BigDouble upgradeCost = GlobalManager.Instance.GetAssistUpgradeCost(order, level, grade);
        if (GlobalManager.Instance.kiwiAmount >= upgradeCost)
        {
            //업그레이드 진행.
            GlobalManager.Instance.kiwiAmount -= upgradeCost;

            if (CanBeUpgradeGrade())
            {
                //승급 처리.
                grade += 1;
                GlobalManager.Instance.assistClickLevelList[order].grade = grade;
            }
            else
            {
                //기존 업그레이드.
                level += 1;
                GlobalManager.Instance.assistClickLevelList[order].level = level;
            }
            
            SetData();
            mainPanel.SetContainers();
            
            EventManager.Instance.TriggerEvent(GameProgressEventType.ASSIST_VIEW_UPGRADE, this);
        }
        else
        {
            UIManager.Instance.PushPanel(UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
        }
        
    }

    public void SetMuteState(bool muted)
    {
        if (linkedMusician != null)
            linkedMusician.Mute(muted);

        buttonMuteImage.color = muted ? Color.red : Color.black;  
    }

    public void OnClickMute()
    {
        bool muteState = !GlobalManager.Instance.assistClickLevelList[order].isMuted;
        GlobalManager.Instance.assistClickLevelList[order].isMuted = muteState;
        SetMuteState(muteState);
    }
}
