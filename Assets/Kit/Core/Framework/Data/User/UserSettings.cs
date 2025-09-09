using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 유저 데이터 저장을 하기 위해 필요한 정적 클래스
/// </summary>
public static class UserSettings
{
    private static readonly string  KEY_USER_ID = "UserId_";
    private static string           userId;

    
    private static readonly ISaveClient Client = new PlayerPrefClient();
    
    /// <summary>
    /// 현재 유저 데이터
    /// </summary>
    public static UserData Data { get; private set; }

    /// <summary>
    /// 유저 데이터 초기화 (앱 실행할 때마다 적용)
    /// </summary>
    public static async void Init()
    {
        try
        {
            //COMMENT : 이후 실제 Player ID를 받아서 사용하도록 처리 예정.
            userId = $"{KEY_USER_ID}_DUMMY";

            //유저 데이터 클래스 생성
            Data = new UserData();
        
            //유저 데이터 저장된 부분이 있다면 불러오기
            await Load();
        }
        catch (Exception e)
        {
            //혹시나 문제 생기면 여기로 들어와라.
            Debug.LogException(e);
        }
    }
    
    /// <summary>
    /// 유저 데이터 저장하기
    /// </summary>
    public static async void Save()
    {   
        try
        {
            UnityEngine.Debug.Log($"<color=red><b>[UserSetting] 유저 데이터 저장하기 완료</b></color>");
            //Json으로 변환
            string json = JsonUtility.ToJson(Data);
            //Json 파일을 저장
            await Client.Save(userId, json);
        }
        catch (Exception e)
        {
            //혹시나 문제 생기면 여기로 들어와라.
            Debug.LogException(e);
        }
        
    }
    
    /// <summary>
    /// 유저 데이터 불러오기
    /// </summary>
    public static async Task Load()
    {  
        try
        {
            UnityEngine.Debug.Log($"<color=yellow><b>[UserSetting] 유저 데이터 불러오기 완료</b></color>");

            string deserializeString = await Client.GetLoadData(userId);
            if (string.IsNullOrEmpty(deserializeString))
                return;
        
            Data = await Client.Load<UserData>(userId);
        }
        catch (Exception e)
        {
            //혹시나 문제 생기면 여기로 들어와라.
            Debug.LogException(e);
        }
        
    }

    /// <summary>
    /// 유저 데이터 삭제
    /// </summary>
    public static async void Delete()
    {
        try
        {
            Data.OnReset();
        
            await Client.Delete(userId);
        }
        catch (Exception e)
        {
            //혹시나 문제 생기면 여기로 들어와라.
            Debug.LogException(e);
        }
    }
    
    /// <summary>
    /// 모든 유저 데이터 삭제
    /// </summary>
    public static async void DeleteAll()
    {
        try
        {
            await Client.DeleteAll();
        }
        catch (Exception e)
        {
            //혹시나 문제 생기면 여기로 들어와라.
            Debug.LogException(e);
        }
       
    }
}
