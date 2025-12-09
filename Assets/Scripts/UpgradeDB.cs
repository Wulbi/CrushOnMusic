using UnityEngine;
using System.Collections.Generic;
using GameLogic.Enum;

[CreateAssetMenu(fileName = "UpgradeDB", menuName = "Kit/Data/UpgradeDB", order = 0)]
public class UpgradeDB : ScriptableObject
{
    public MainUpgradeData          mainData;
    public List<AssistUpgradeData>  assistDataList;
    
    [System.Serializable]   //인스펙터에 보여진다.
    public class MainUpgradeData
    {
        public Sprite icon;
        public string Name;
    }
    
    [System.Serializable]
    public class AssistUpgradeData
    {
        public Sprite icon;
        public string Name;

        public List<AssistGradeData> gradeDataList;
        
        [Header("Loop Audio Clips")]
        [Tooltip("The type of loop clip for this assist container.")]
        public LoopClipType loopClipType;
        
        [Tooltip("Audio clips for this assist container. First clip is used by default.")]
        public List<AudioClip> loopClips = new List<AudioClip>();
    }

    [System.Serializable]
    public class AssistGradeData
    {
        public int needLevel;
        public int increaseUpgradeCost;
    }
}
