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
    public bool isUpgraded = false;

    public MainPanel mainPanel;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public LoopClipType[] loopClipTypes;

    [Header("Tick")]
    private float nextUiTick;              // UI 업데이트 간격용
    private const float UiTick = 0.15f;    // 매 프레임 갱신 대신 0.15초 주기

    public UpgradeDB.AssistUpgradeData Data 
    {
        get
        {
            if (DatabaseManager.Instance == null || DatabaseManager.Instance.upgradeDB == null || 
                DatabaseManager.Instance.upgradeDB.assistDataList == null)
            {
                Debug.LogError($"[AssistContainer] DatabaseManager or UpgradeDB not available for order {order}");
                return null;
            }
            
            if (order < 0 || order >= DatabaseManager.Instance.upgradeDB.assistDataList.Count)
            {
                Debug.LogError($"[AssistContainer] Order {order} is out of bounds. Database has {DatabaseManager.Instance.upgradeDB.assistDataList.Count} assists.");
                return null;
            }
            
            return DatabaseManager.Instance.upgradeDB.assistDataList[order];
        }
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
        
        // Start loop playback if upgraded
        if (isUpgraded)
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

        bool affordable = !isUpgraded && gm.likesAmount >= gm.GetAssistUpgradeCost(order);

        // 버튼 색상 통일 적용
        UIThemeUtil.SetUpgradeButtonVisual(buttonImage, affordable, false);

        // 버튼 상호작용은 색과 별개로 명확히
        if (buttonUpgrade && buttonUpgrade.interactable != affordable)
            buttonUpgrade.interactable = affordable;

        // 레벨 라벨 - show "Upgrade" if not upgraded, hide if upgraded
        var newLevelText = isUpgraded ? "Complete" : "Upgrade";
        if (labelLevel.text != newLevelText) labelLevel.text = newLevelText;
    }

    public void SetData()
    {
        var data = Data;
        if (data == null)
        {
            Debug.LogError($"[AssistContainer] Cannot set data for order {order} - Data is null");
            return;
        }
        
        Icon.sprite     = data.icon;
        labelName.text  = data.Name;
        
        if (isUpgraded)
        {
            labelDesc.text  = $"{GlobalManager.Instance.GetAssistAmount(order)} Likes /s";
            labelCost.text  = ""; // No cost if already upgraded
        }
        else
        {
            labelDesc.text  = "0 Likes /s";
            labelCost.text  = $"+{GlobalManager.Instance.GetAssistUpgradeCost(order)}";
        }
        
        // Set up audio clip for this assist container
        SetAudioClip(order);
        
        // Stop playback if not upgraded
        if (!isUpgraded)
        {
            StopLoopPlayback();
        }
    }
    
    public void SetAudioClip(int order)
    {
        // Initialize AudioSource if needed
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Get audio clip directly from UpgradeDB
        AudioClip clip = null;
        if (Data != null && Data.loopClips != null && Data.loopClips.Count > 0)
        {
            // Use first clip (index 0) by default
            clip = Data.loopClips[0];
        }
        
        // Fallback to loopClipTypes array if UpgradeDB doesn't have clips (backward compatibility)
        // Note: Can't use CommonSounds anymore since loop clips are removed from it
        if (clip == null && loopClipTypes != null && loopClipTypes.Length > 0 && order >= 0 && order < loopClipTypes.Length)
        {
            Debug.LogWarning($"[AssistContainer] UpgradeDB에 loopClips가 없습니다. order: {order}, UpgradeDB에 loopClips를 설정해주세요.");
        }

        if (clip != null && isUpgraded)
        {
            audioSource.clip = clip;
        }
        else
        {
            // Clear the audio clip when not upgraded or clip is null
            audioSource.clip = null;
            if (!isUpgraded)
            {
                Debug.Log($"[AssistContainer] 컨테이너가 업그레이드되지 않았습니다. order: {order}");
            }
            else
            {
                Debug.LogWarning($"[AssistContainer] 음악 클립이 존재하지 않습니다. order: {order}, UpgradeDB에 loopClips를 설정해주세요.");
            }
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
        // Only start playback if upgraded and we have a valid clip
        if (audioSource != null && audioSource.clip != null && isUpgraded)
        {
            audioSource.loop = true; // Set to loop for continuous playback
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (!isUpgraded)
        {
            // Stop playback if not upgraded
            StopLoopPlayback();
        }
    }
    
    public void StartLoopPlaybackIfNeeded()
    {
        // Only start if not already playing and conditions are met
        if (audioSource != null && audioSource.clip != null && isUpgraded && !audioSource.isPlaying)
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
        
        // Don't allow upgrading if already upgraded
        if (isUpgraded)
        {
            return;
        }
        
        BigDouble cost = gm.GetAssistUpgradeCost(order);

        if (gm.likesAmount < cost)
        {
            UIManager.Instance.PushPanel(UIPanelType.POPUP_PANEL, "Warning", "You need more Likes!");
            return;
        }

        gm.likesAmount -= cost;

        // Mark this container as upgraded
        isUpgraded = true;
        gm.assistClickLevelList[order].isUpgraded = true;
        
        // Update assist state bools when upgraded
        gm.UpdateAssistStates();
        
        // Note: The next container will be opened (made visible) by GetActiveAssistCount() logic

        SetData();
        mainPanel.SetContainers();
        EventManager.Instance.TriggerEvent(GameProgressEventType.ASSIST_VIEW_UPGRADE, this);

        // 업그레이드 직후 색/문구 최신화
        ApplyVisuals(force:true);
        
        // Save data immediately after upgrade to persist changes
        GlobalManager.Instance.SaveData();
    }

    public void SetMuteState(bool muted)
    {
        MuteMusic(muted);
        UIThemeUtil.SetMuteVisual(buttonMuteImage, muted);
        
        // Explicit color cue: red when muted, white when unmuted
        if (buttonMuteImage != null)
        {
            buttonMuteImage.color = muted ? Color.red : Color.white;
        }
    }

    public void OnClickMute()
    {
        bool muteState = !GlobalManager.Instance.assistClickLevelList[order].isMuted;
        GlobalManager.Instance.assistClickLevelList[order].isMuted = muteState;
        SetMuteState(muteState);
    }
}
