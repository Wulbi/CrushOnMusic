using System;
using UnityEngine;
using BigNumber;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using GameLogic.Enum;

[System.Serializable]
public class AssistData
{
    public bool isUpgraded;
    public bool isMuted;
}

public class MusicianData
{
    public int level;
    public AudioClip Clip;
}
public class GlobalManager : SingletonBehaviour<GlobalManager>
{
    /// <summary>
    /// 키위 수량
    /// </summary>
    public BigDouble likesAmount;
    
    /// <summary>   
    /// 클릭 레벨
    /// </summary>
    public int clickLevel = 1;

    public List<AssistData> assistClickLevelList = new List<AssistData>();
    
    public List<MusicianData> musicList = new List<MusicianData>();

    private float lastUpdateTime = 0f;

    /// <summary>
    /// Tracks which loop clip types are currently active (awake and upgraded)
    /// </summary>
    public HashSet<LoopClipType> activeLoopTypes = new HashSet<LoopClipType>();

    /// <summary>
    /// Assist container state bools - tracks which assists are on (awake and upgraded)
    /// These are derived from activeLoopTypes for backward compatibility
    /// </summary>
    public bool isDrumOn = false;
    public bool isBassOn = false;
    public bool isGuitarOn = false;
    public bool isGuitar2On = false;
    public bool isPianoOn = false;
    public bool isChoirOn = false;
    public bool isFluteOn = false;
    public bool isKeyboardOn = false;
    public bool isPiano2On = false;

    /// <summary>
    /// 피버 상태 체크 - 게임 내 피버 모드 활성화 여부를 나타냅니다.
    /// </summary>
    public bool IsFever = false;

    /// <summary>
    /// 첫 실행 여부를 나타내는 플래그입니다.
    /// </summary>
    public bool isFirst;    
    /// <summary>
    /// 초당 키위 획득량 값 반환
    /// </summary>
    public BigDouble GetKPS()
    {
        BigDouble result = 0;
        for (int i = 0; i < assistClickLevelList.Count; i++)
        {
            if (!assistClickLevelList[i].isUpgraded)
                continue;

            result += GetAssistAmount(i);
        }

        return result;
    }

    private void Update()
    {
        //초당 키위를 획득하는 코드를 넣어준다.
        lastUpdateTime += Time.deltaTime;
        if (lastUpdateTime >= 1f)
        {
            lastUpdateTime = 0f;
            likesAmount += GetKPS();
        }
    }

    public void ResetData()
    {
        // Reset all data to default values
        likesAmount = 0;
        clickLevel = 1;
        assistClickLevelList.Clear();
        IsFever = false;
        
        // Initialize assists with default values
        // First container (Drum) is always visible but starts as NOT upgraded
        int databaseAssistCount = DatabaseManager.Instance.upgradeDB.assistDataList.Count;
        for (int i = 0; i < databaseAssistCount; i++)
        {
            assistClickLevelList.Add(new AssistData()
            {
                isUpgraded = false, // All containers start as not upgraded (Drum is always visible but not upgraded)
                isMuted = false
            });
        }
        
        // Update assist state bools after reset
        UpdateAssistStates();
    }

    public void LoadData()
    {
        // Ensure UserSettings.Data is initialized
        if (UserSettings.Data == null)
        {
            Debug.LogWarning("[GlobalManager] UserSettings.Data is null, initializing...");
            UserSettings.Init();
            return; // Will be called again after Init completes
        }
        
        //저장된 유저 데이터를 게임 데이터로 변환한다.
        //1. 키위 수량 데이터 불러오기
        likesAmount = UserSettings.Data.coin;

        //2. 터치 레벨 데이터 불러오기
        // Ensure clickLevel is at least 1 (default value)
        clickLevel = UserSettings.Data.baseLevel > 0 ? UserSettings.Data.baseLevel : 1;
        
        assistClickLevelList.Clear();
        
        //3. 보조 장치 데이터 불러오기
        int databaseAssistCount = DatabaseManager.Instance.upgradeDB.assistDataList.Count;
        
        // Always initialize all assists based on database count
        for (int i = 0; i < databaseAssistCount; i++)
        {
            // If we have saved data for this assist, use it; otherwise initialize to default
            if (UserSettings.Data.assistContents != null && 
                i < UserSettings.Data.assistContents.Count)
            {
                assistClickLevelList.Add(new AssistData()
                {
                    isUpgraded = UserSettings.Data.assistContents[i].isUpgraded,
                    isMuted = UserSettings.Data.assistContents[i].isMuted
                });
            }
            else
            {
                // Initialize new assist with default values
                // First container (Drum) is always visible but starts as not upgraded
                assistClickLevelList.Add(new AssistData()
                {
                    isUpgraded = false, // All containers start as not upgraded
                    isMuted = false
                });
            }
        }
        
        //4. 업적 데이터 불러오기 -> Manager에서 처리
        
        // Update assist state bools after loading data
        UpdateAssistStates();
    }

    public void SaveData()
    {
        //게임 데이터를 유저 데이터로 변환 -> 저장.
        //1. 키위 수량 저장
        UserSettings.Data.coin = likesAmount;
        //2. 터치 레벨 저장
        UserSettings.Data.baseLevel = clickLevel;
        //3. 보조 장치 데이터 저장
        // Ensure assistContents list is properly sized
        if (UserSettings.Data.assistContents == null)
        {
            UserSettings.Data.assistContents = new List<UserAssistData>();
        }
        
        // Clear and rebuild the list to ensure it matches assistClickLevelList
        UserSettings.Data.assistContents.Clear();
        for (int i = 0; i < assistClickLevelList.Count; i++)
        {
            UserSettings.Data.assistContents.Add(new UserAssistData()
            {
                itemId = i,
                isUpgraded = assistClickLevelList[i].isUpgraded,
                isMuted = assistClickLevelList[i].isMuted
            });
        }
        
        //4.업적데이터 저장
        if (AchievementManager.HasInstance)
        {
            UserSettings.Data.achievementStates = AchievementManager.Instance.achievementList.ConvertAll(state => new UserAchievementData()
            {
                type = state.Type,
                currentCount = state.CurrentCount,
                level = state.Level,
                isReward = state.IsReward
            });
        }
        
        //JSON 으로 변환 후 저장.
        UserSettings.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //유니티 게임이 백그라운드로 넘어갔다
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        //게임이 종료될 때
        SaveData();
    }


    /// <summary>
    /// 레벨이 증가함에 따라 키위 증가량 변화
    /// </summary>
    /// <returns></returns>
    public BigDouble GetTouchAmount()
    {
        BigDouble amt = clickLevel + 1;
       //return amt;
        return IsFever ? amt * 2 : amt;
    }

    /// <summary>
    /// 업그레이드 코스트 (가격)
    /// </summary>
    /// <returns></returns>
    public BigDouble GetUpgradeCost()
    {
        return clickLevel * 10;
    }

    public BigDouble GetAssistUpgradeCost(int order)
    {
        BigDouble baseCost = 10 * BigDouble.Pow(2, order + 1);
        return BigDouble.Round(baseCost);
    }

    public BigDouble GetAssistAmount(int order)
    {
        BigDouble baseAmount = 1 * BigDouble.Pow(1.5f, order + 1);
        return BigDouble.Round(baseAmount);
    }

    /// <summary>
    /// Checks if a specific loop clip type is currently active (awake and upgraded)
    /// </summary>
    public bool IsLoopOn(LoopClipType type)
    {
        return activeLoopTypes.Contains(type);
    }

    /// <summary>
    /// Updates assist container state based on which assists are awake and upgraded
    /// Uses LoopClipType enum from UpgradeDB instead of string-based name matching
    /// </summary>
    public void UpdateAssistStates()
    {
        // Clear active loop types
        activeLoopTypes.Clear();

        if (DatabaseManager.Instance == null || DatabaseManager.Instance.upgradeDB == null || 
            DatabaseManager.Instance.upgradeDB.assistDataList == null)
        {
            // Reset boolean fields when database is unavailable
            UpdateBooleanFieldsFromActiveLoops();
            return;
        }

        var assistDataList = DatabaseManager.Instance.upgradeDB.assistDataList;
        
        // Check each assist - it's "on" if it's awake (visible) AND upgraded
        for (int i = 0; i < assistClickLevelList.Count && i < assistDataList.Count; i++)
        {
            // Check if assist is awake (visible)
            // First assist (i == 0) is always awake
            // Subsequent assists are awake if the previous one is upgraded
            bool isAwake = (i == 0) || (i > 0 && assistClickLevelList[i - 1].isUpgraded);
            
            // Assist loop is "on" only if it's both awake AND upgraded
            bool isOn = isAwake && assistClickLevelList[i].isUpgraded;
            
            if (isOn)
            {
                // Get the loop clip type from the assist data
                LoopClipType loopType = assistDataList[i].loopClipType;
                
                // Add to active loop types if it's a valid enum value
                // (Assuming all enum values are valid, but we could add validation if needed)
                activeLoopTypes.Add(loopType);
            }
        }
        
        // Update boolean fields from activeLoopTypes for backward compatibility
        UpdateBooleanFieldsFromActiveLoops();
    }

    /// <summary>
    /// Updates the legacy boolean fields from activeLoopTypes for backward compatibility
    /// </summary>
    private void UpdateBooleanFieldsFromActiveLoops()
    {
        isDrumOn = activeLoopTypes.Contains(LoopClipType.DRUM);
        isBassOn = activeLoopTypes.Contains(LoopClipType.BASS);
        isGuitarOn = activeLoopTypes.Contains(LoopClipType.GUITAR1);
        isGuitar2On = activeLoopTypes.Contains(LoopClipType.GUITAR2);
        isPianoOn = activeLoopTypes.Contains(LoopClipType.PIANO);
        isChoirOn = activeLoopTypes.Contains(LoopClipType.CHOIR);
        isFluteOn = activeLoopTypes.Contains(LoopClipType.FLUTE);
        isKeyboardOn = activeLoopTypes.Contains(LoopClipType.KEYBOARD);
        isPiano2On = activeLoopTypes.Contains(LoopClipType.PIANO2);
    }
}
