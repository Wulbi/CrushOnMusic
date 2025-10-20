using System;
using BigNumber;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameLogic.Enum;

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
    
    [Header("Audio")]
    public AudioSource audioSource;
    public LoopClipType[] loopClipTypes;

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
        // Initialize AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        // If still null, add AudioSource component
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        // 최초 색/라벨 일괄 적용
        UIThemeUtil.SetLabel(labelName);
        UIThemeUtil.SetLabel(labelDesc, isSub:true);
        UIThemeUtil.SetLabel(labelCost);
        UIThemeUtil.SetLabel(labelLevel);

        SetData();
        ApplyVisuals(force:true);
        
        // Start loop playback if level > 0
        if (level > 0)
        {
            StartLoopPlaybackIfNeeded();
        }
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

        bool affordable = gm.likesAmount >= gm.GetAssistUpgradeCost(order, level, grade);
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
        
        // Set up audio clip for this assist container
        SetAudioClip(order, level, grade);
        
        // Stop playback if level is 0
        if (level <= 0)
        {
            StopLoopPlayback();
        }
    }
    
    public void SetAudioClip(int order, int level, int grade)
    {
        // Initialize AudioSource if needed
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (loopClipTypes == null || loopClipTypes.Length == 0)
        {
            Debug.LogWarning($"[AssistContainer] loopClipTypes 배열이 설정되지 않았습니다. Unity Inspector에서 설정해주세요.");
            return;
        }
        
        if (order >= 0 && order < loopClipTypes.Length)
        {
            LoopClipType type = loopClipTypes[order];
            AudioClip clip = CommonSounds.GetClip(type, grade);

            if (clip != null && level > 0)
            {
                audioSource.clip = clip;
            }
            else
            {
                // Clear the audio clip when level is 0 or clip is null
                audioSource.clip = null;
                if (level <= 0)
                {
                    Debug.Log($"[AssistContainer] 레벨이 0입니다. order: {order}");
                }
                else
                {
                    Debug.LogWarning($"[AssistContainer] 음악 클립이 존재하지 않습니다. LoopClipType: {type}, Grade: {grade}");
                }
            }
        }
        else
        {
            Debug.LogError($"[AssistContainer] order 인덱스가 loopClipTypes 범위를 벗어났습니다. order: {order}, 배열 길이: {loopClipTypes.Length}");
        }
    }
    
    public void PlayMusic()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.loop = true; // Ensure looping is enabled
            audioSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    public void StartLoopPlayback()
    {
        // Only start playback if level > 0 and we have a valid clip
        if (audioSource != null && audioSource.clip != null && level > 0)
        {
            audioSource.loop = true; // Set to loop for continuous playback
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (level <= 0)
        {
            // Stop playback if level is 0 or below
            StopLoopPlayback();
        }
    }
    
    public void StartLoopPlaybackIfNeeded()
    {
        // Only start if not already playing and conditions are met
        if (audioSource != null && audioSource.clip != null && level > 0 && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    public void StopLoopPlayback()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
    
    public void MuteMusic(bool shouldMute)
    {
        if (audioSource != null)
        {
            audioSource.mute = shouldMute;
        }
    }

    public void OnClickedUpgrade()
    {
        var gm = GlobalManager.Instance;
        BigDouble cost = gm.GetAssistUpgradeCost(order, level, grade);

        if (gm.likesAmount < cost)
        {
            UIManager.Instance.PushPanel(UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
            return;
        }

        gm.likesAmount -= cost;

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
        MuteMusic(muted);
        UIThemeUtil.SetMuteVisual(buttonMuteImage, muted);
    }

    public void OnClickMute()
    {
        bool muteState = !GlobalManager.Instance.assistClickLevelList[order].isMuted;
        GlobalManager.Instance.assistClickLevelList[order].isMuted = muteState;
        SetMuteState(muteState);
    }
}
