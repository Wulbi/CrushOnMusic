using System.Collections.Generic;
using System;
using BigNumber;

/// <summary>
/// 유저 데이터 클래스
/// </summary>
[Serializable]
public class UserData
{
    /// <summary>
    /// 저장된 코인
    /// </summary>
    public BigDouble        coin;
    
    /// <summary>
    /// 저장된 기본 터치 레벨
    /// </summary>
    public int              baseLevel;
    
    /// <summary>
    /// 저장된 보조 상태 클래스 리스트
    /// </summary>
    public List<UserAssistData> assistContents;
    
    /// <summary>
    /// 저장된 효과음 볼륨
    /// </summary>
    public float            sfxVolume;               //Sound Effect
    
    /// <summary>
    /// 저장된 배경음 볼륨
    /// </summary>
    public float            musicVolume;             //Music
    
    /// <summary>
    /// 최초 실행한 상태인가?
    /// </summary>
    public bool             isFirst;

    /// <summary>
    /// 보석 재화
    /// </summary>
    public int diamond;
    
    /// <summary>
    /// 백그라운드 ID
    /// </summary>
    public int selectedBackgroundId;
    
    /// <summary>
    /// 업적 데이터
    /// </summary>
    public List<UserAchievementData> achievementStates;
    
    
    
    //COMMENT: 저장할 데이터 변수 선언을 해줍니다. 

    public UserData()
    {
        coin                    = 0;
        diamond  = 0;
        baseLevel               = 0;
        sfxVolume               = 1f;
        musicVolume             = 1f;
        selectedBackgroundId = 0;
        
        assistContents      = new List<UserAssistData>();
        isFirst             = true;
        
        achievementStates = new List<UserAchievementData>();
        //COMMENT: 저장할 데이터 변수 초기화 해줍니다.
        //초기화 값은 우리가 데이터를 지우거나 아무것도 없는 초기 상태에서 사용됩니다.
    }

    /// <summary>
    /// 디버그 출력용 메소드
    /// </summary>
    /// <returns>현재 저장 상태 출력용 문자열</returns>
    public string toString()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        stringBuilder.Append($"coin : {coin.ToCustomString()}\n");
        stringBuilder.Append($"baseLevel : {baseLevel.ToString()}\n");
        stringBuilder.Append($"sfxVolume : {sfxVolume.ToString("F1")}\n");
        stringBuilder.Append($"musicVolume : {musicVolume.ToString("F1")}\n");
        stringBuilder.Append($"isFirst : {isFirst.ToString()}\n");
      
        for (int i = 0; i < assistContents.Count; i++)
            stringBuilder.Append($"assistContents[{i}] : {assistContents[i].toString()}\n");
        
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 초기화할 때 이 함수를 사용한다
    /// </summary>
    public void OnReset()
    {
        coin                    = 0;
        diamond                 = 0;
        baseLevel               = 0;
        sfxVolume               = 1f;
        musicVolume             = 1f;
        selectedBackgroundId = 0;
        
        assistContents          .Clear();
        isFirst             = true;
        
        achievementStates.Clear();
    }
}
/// <summary>
/// 콘텐츠 데이터 저장용 클래스
/// </summary>
[Serializable]
public class UserContentsData
{
    /// <summary>
    /// 콘텐츠 내 아이템 ID
    /// </summary>
    public int  itemId;
    
    /// <summary>
    /// 콘텐츠 내 아이템 LEVEL (해당 레벨을 통해서 계산한다)
    /// </summary>
    public int  level;

    public UserContentsData()
    {
        itemId              = 0;
        level               = 0;
    }

    /// <summary>
    /// 디버그 출력용 메소드
    /// </summary>
    /// <returns>현재 저장 상태 출력용 문자열</returns>
    public virtual string toString()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        stringBuilder.Append($"itemId : {itemId.ToString()}\n");
        stringBuilder.Append($"level : {level.ToString()}\n");

        return stringBuilder.ToString();
    }
}

/// <summary>
/// 보조 콘텐츠 데이터 저장용 클래스
/// </summary>
[Serializable]
public class UserAssistData : UserContentsData
{
    /// <summary>
    /// 보조 등급
    /// </summary>
    public int grade;

    public bool isMuted;
    
    public UserAssistData() : base() 
    { 
        grade       = 0;
        isMuted = false;
    }

    public override string toString()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        stringBuilder.Append($"itemId : {itemId.ToString()}\n");
        stringBuilder.Append($"level : {level.ToString()}\n");
        stringBuilder.Append($"grade : {grade.ToString()}\n");
        return stringBuilder.ToString();
    }
}

[Serializable]
public class UserAchievementData
{
    public AchievementType type;
    public bool isReward;
    public int currentCount;
    public int level;
}