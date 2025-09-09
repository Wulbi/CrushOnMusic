using System;
using UnityEngine;
using BigNumber;
using System.Collections.Generic;

[System.Serializable]
public class AssistData
{
    public int level;
    public int grade;
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
    public BigDouble kiwiAmount;
    
    /// <summary>
    /// 클릭 레벨
    /// </summary>
    public int clickLevel = 1;

    public List<AssistData> assistClickLevelList = new List<AssistData>();
    
    public List<MusicianData> musicList = new List<MusicianData>();

    /// <summary>
    /// 피버 상태 체크
    /// </summary>
    public bool IsFever = false;

    public GameObject background;
    
    private float lastUpdateTime = 0f;
    /// <summary>
    /// 초당 키위 획득량 값 반환
    /// </summary>
    public BigDouble GetKPS()
    {
        BigDouble result = 0;
        for (int i = 0; i < assistClickLevelList.Count; i++)
        {
            if (assistClickLevelList[i].level <= 0)
                continue;

            result += GetAssistAmount(i, assistClickLevelList[i].level);
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
            kiwiAmount += GetKPS();
        }
    }

    public void LoadData()
    {
        //저장된 유저 데이터를 게임 데이터로 변환한다.
        //1. 키위 수량 데이터 불러오기
        kiwiAmount = UserSettings.Data.coin;

        //2. 터치 레벨 데이터 불러오기
        clickLevel = UserSettings.Data.baseLevel;
        
        assistClickLevelList.Clear();
        
        //3. 보조 장치 데이터 불러오기
        if (UserSettings.Data.assistContents.Count <= 0)
        {
            //보조 장치 데이터 초기화 (아무것도 저장 X)
            for (int i = 0; i < DatabaseManager.Instance.upgradeDB.assistDataList.Count; i++)
            {
                //게임에 사용하는 보조 데이터.
                assistClickLevelList.Add(new AssistData()
                {
                    level = 0,
                    grade = 0,
                    isMuted = false
                });
            }
        }
        else
        {
            for (int i = 0; i < UserSettings.Data.assistContents.Count; i++)
            {
                assistClickLevelList.Add(new AssistData()
                {
                    level = UserSettings.Data.assistContents[i].level,
                    grade = UserSettings.Data.assistContents[i].grade,
                    isMuted = UserSettings.Data.assistContents[i].isMuted
                });
            }
        }
        
        //4. 업적 데이터 불러오기 -> Manager에서 처리
        
    }

    public void SaveData()
    {
        //게임 데이터를 유저 데이터로 변환 -> 저장.
        //1. 키위 수량 저장
        UserSettings.Data.coin = kiwiAmount;
        //2. 터치 레벨 저장
        UserSettings.Data.baseLevel = clickLevel;
        //3. 보조 장치 데이터 저장
        if (UserSettings.Data.assistContents.Count <= 0)
        {
            for (int i = 0; i < assistClickLevelList.Count; i++)
            {
                UserSettings.Data.assistContents.Add(new UserAssistData()
                {
                    itemId = i,
                    grade = assistClickLevelList[i].grade,
                    level = assistClickLevelList[i].level,
                    isMuted  = assistClickLevelList[i].isMuted
                });
            }
        }
        else
        {
            for (int i = 0; i < assistClickLevelList.Count; i++)
            {
                UserSettings.Data.assistContents[i].itemId = i;
                UserSettings.Data.assistContents[i].level = assistClickLevelList[i].level;
                UserSettings.Data.assistContents[i].grade = assistClickLevelList[i].grade;
                UserSettings.Data.assistContents[i].isMuted = assistClickLevelList[i].isMuted;
            }
        }
        //4.업적데이터 저장
        UserSettings.Data.achievementStates = AchievementManager.Instance.achievementList.ConvertAll(state => new UserAchievementData()
        {
            type = state.Type,
            currentCount = state.CurrentCount,
            level = state.Level,
            isReward = state.IsReward
        });
        
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

    public BigDouble GetAssistUpgradeCost(int order, int lv, int grade)
    {
        BigDouble baseCost = 10 * BigDouble.Pow(2, order + 1);
        int maxGrade = Mathf.Min(DatabaseManager.Instance.upgradeDB.assistDataList[order].gradeDataList.Count -1, grade);
        float increaseAmt = grade - 1 < 0
            ? 1f
            : DatabaseManager.Instance.upgradeDB.assistDataList[order].gradeDataList[maxGrade].increaseUpgradeCost;
        return BigDouble.Round((baseCost * increaseAmt) * BigDouble.Pow(1.2f, lv));
    }

    public BigDouble GetAssistAmount(int order, int lv)
    {
        BigDouble baseAmount = 1 * BigDouble.Pow(1.5f, order + 1);
        return BigDouble.Round(baseAmount * BigDouble.Pow(lv, 1.15f));
    }
}
