using System;
using System.Threading.Tasks;
using SimpleLocalization;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private string userId;
    private int langIdx;
    private async Task SetLoginGuest()
    {
        try
        {
            //1. 유니티 서비스 초기화
            await UnityServices.InitializeAsync();
            //2. 구글, 애플 로그인, 익명 로긘 시킴
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); //익명 로그인
        
            //playerID 유니티 서비스 받아옴
            userId = AuthenticationService.Instance.PlayerId;
            Debug.Log(AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
        
    }

    private async void SetData()
    {
        await SetLoginGuest();
        
        // 유저 불러오기
        
        //플러그인 동기화
        
        //게임시작 버튼 활성화
    }
    void Start()
    {
        
        
        
        //1. 언어 관리 클래스 초기화
        Localizator.Initialize();
        
        //2. 언어 선택
        Localizator.ChangeLanguage(SystemLanguage.Korean);
        langIdx = (int) SystemLanguage.Korean;
        //Localizator.ChangeLanguage((SystemLanguage) langIdx);
        //Application.systemLanguage - 유저 설정 언어
        Debug.Log(Localizator.Translate("UI_TEXT_LOGO_TITLE"));
    }

    
}
