using UnityEngine;

/// <summary>
/// 업적에 대한 내용을 가공해준다
/// </summary>
public class AchievementState
{
    public AchievementType              Type;
    public AchievementDB.Achievement    Data;
    public int                          CurrentCount;
    public bool                         IsReward;
    public int                          Level;

    public bool IsDone()
    {
        return Level >= Data.GoalCount.Length;
    }

    public int GetMaxLevel()
    {
        return Data.GoalCount.Length - 1;
    }
    
    public AchievementState(AchievementDB.Achievement data, int currentCount, int level)
    {
        Data = data;
        CurrentCount = currentCount;
        Level = level;

        Type = data.Type;

        CheckReward();
    }
    /// <summary>
    /// 진행도를 체크해주는 함수.
    /// </summary>
    public void ProgressAndTryCount(int count)
    {
        //진행도 누적.
        CurrentCount += count;
        //보상 트리거 확인.
        CheckReward();
        
        //업적 업데이트 이벤트 발송.
        EventManager.Instance.TriggerEvent(GameProgressEventType.ACHIEVEMENT_UPDATED, this);
    }

    public void ChangeCount(int count)
    {
        CurrentCount = count;
        
        CheckReward();
        
        EventManager.Instance.TriggerEvent(GameProgressEventType.ACHIEVEMENT_UPDATED, this);
        
    }
    public void CheckReward()
    {
        int maxLv = IsDone() ? GetMaxLevel() : Level;
        IsReward = CurrentCount >= Data.GoalCount[maxLv];
    }

    public void Reward()
    {
        if (IsDone())
            return;
        
        if (!IsReward)
            return;

        Level++;
        CheckReward();
        
        //업적 업데이트 이벤트 발송.
        EventManager.Instance.TriggerEvent(GameProgressEventType.ACHIEVEMENT_UPDATED, this);
    }
}
