using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AchievementManager : SingletonBehaviour<AchievementManager>
{
    //1. 가공 클래스들을 다 가지고 있다.
    public List<AchievementState> achievementList = new List<AchievementState>();

    public void InitData()
    {
        achievementList.Clear();

        var savedStates = UserSettings.Data.achievementStates;

        foreach (var achData in DatabaseManager.Instance.achievementDB.achievements)
        {
            var saved = savedStates.Find(a => a.type == achData.Type);
            if (saved != null)
            {
                achievementList.Add(
                    new AchievementState(achData, saved.currentCount, saved.level));
            }
            else
            {
                achievementList.Add(
                    new AchievementState(achData, 0, 0));
            }
        }
    }

    public void CheckAchievement(AchievementType type, int count)
    {
        //1. 어떤 식으로 가공 클래스를 구별해서 가져올건가?
        AchievementState state = achievementList.Find(a => a.Type == type);
        if (state == null)
            return;
        
        //2. State 여기에서 진행도를 체크하는 함수
        state.ProgressAndTryCount(count);
    }
    
    public void ChangeAchievement(AchievementType type, int count)
    {
        //1. 어떤 식으로 가공 클래스를 구별해서 가져올건가?
        AchievementState state = achievementList.Find(a => a.Type == type);
        if (state == null)
            return;
        
        //2. State 여기에서 진행도를 체크하는 함수
        state.ChangeCount(count);
    }
    //보상 받을게 있는지?
    public bool GetRewardStatus()
    {
        return achievementList.Any(state => !state.IsDone() && state.IsReward);
    }
}
