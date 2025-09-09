using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AchievementDB", menuName = "Kit/Data/AchievementDB", order = 1)]
public class AchievementDB : ScriptableObject
{
    public List<Achievement> achievements;
    
    [Serializable]
    public class Achievement
    {
        public AchievementType  Type;
        public Sprite           Icon;
        public string Name;
        public string Desc;
        public int[] GoalCount;
        public int[] RewardCount;
        public Sprite Background;
    
    }
}
