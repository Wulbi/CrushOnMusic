using System;
using UnityEngine;
using UnityEngine.UI;

// 피버 바(FeverBar)는 게임 내에서 피버 게이지의 상태를 관리하고 UI에 반영하는 역할을 합니다.
// 피버 상태는 게이지가 가득 차면 활성화되고, 일정 시간 후 다시 비활성화됩니다.
// 게이지는 시간이 지남에 따라 감소하며, 특정 이벤트로 증가할 수 있습니다.
public class FeverBar : MonoBehaviour
{
    /// <summary>
    /// 피버 액션 타입
    /// </summary>
    public enum FeverActionType
    {
        NONE,                               // 비활성 상태(일반 모드)
        ACTIVE,                             // 활성 상태(피버 모드)
    }

    /// <summary>
    /// 피버 게이지 표시하는 부분
    /// </summary>
    public Image            feverFillImage; 
    
    public FeverActionType  ActionType;
    public float            FeverSecondsLeft;
    
    private float           currentFeverValue = 0f;
    private float           currentDecreaseRate;
    private float           currentIncreaseAmount;
    private float           lastUpdateTime;
    private float           maxFeverValue;
    private float           feverTime;
    private float           accelerationFactor;
    private float           maxIncreaseRate;
    private float           maxDecreaseRate;
    private float           initialDecreaseRate;
    private float           initialIncreaseAmount;
    private bool            isInitialized = false;
    
    private void Update()
    {
        // 초기화가 안 됐으면 동작하지 않음
        if (!isInitialized)
            return;
        
        // 피버 상태 처리
        HandleFever();
        
        // UI에 피버 게이지 반영
        SetFeverFill(GetFeverRate());
    }

    public float GetFeverRate()
    {
        // 피버 게이지 비율(0~1) 반환
        return currentFeverValue <= 0 ? 0f : currentFeverValue / maxFeverValue;
    }

    public void Init()
    {
        // 피버 바 초기 설정값 지정
        feverTime               = 12f;   // 피버 지속 시간 12초
        initialDecreaseRate     = 5f;    // 초기 감소 속도
        initialIncreaseAmount   = 5f;    // 초기 증가량
        maxFeverValue           = 100f;  // 최대 게이지 값
        accelerationFactor      = 1f;    // 가속도 계수(1이면 가속 없음)
        maxDecreaseRate         = 7f;    // 최대 감소 속도
        maxIncreaseRate         = 7f;    // 최대 증가 속도
        
        currentDecreaseRate     = initialDecreaseRate;
        currentIncreaseAmount   = initialIncreaseAmount;
        currentFeverValue       = 0f;
        lastUpdateTime          = (float)Time.realtimeSinceStartup;
        isInitialized           = true; // 초기화 완료
    }

    private void HandleFever()
    {
        // 피버 상태에 따라 동작 분기
        switch (ActionType)
        {
            case FeverActionType.NONE:
                {
                    // 게이지가 다 찼을 때 피버 발동
                    if (currentFeverValue >= maxFeverValue)
                    {
                        lastUpdateTime      = (float)Time.realtimeSinceStartup; // 피버 시작 시간 기록
                        ActionType          = FeverActionType.ACTIVE;           // 피버 모드로 전환
                        currentFeverValue   = maxFeverValue;                    // 게이지 최대치 고정

                        //COMMENT : 피버 상태 ON
                        GlobalManager.Instance.IsFever = true;
                        EventManager.Instance.TriggerEvent(GameProgressEventType.FEVER_UPDATED, this);
                        return;
                    }
     
                    // 피버가 발동되지 않았을 때만 게이지 감소
                    DecreaseFever();
                }
                break;
            case FeverActionType.ACTIVE:
                {
                    // 피버 모드 남은 시간 계산
                    FeverSecondsLeft = feverTime - (float)(Time.realtimeSinceStartup - lastUpdateTime);
                    
                    // 피버 시간이 아직 남아있으면 대기
                    if (Time.realtimeSinceStartup - (double)lastUpdateTime < feverTime)
                        return;
          
                    // 피버 종료: 게이지 초기화 및 상태 복귀
                    currentFeverValue       = 0;
                    lastUpdateTime          = (float)Time.realtimeSinceStartup;
                    currentDecreaseRate     = initialDecreaseRate;
                    currentIncreaseAmount   = initialIncreaseAmount;
                    
                    // 다시 기본 모드로 전환
                    ActionType              = FeverActionType.NONE;
                    lastUpdateTime          = (float)Time.realtimeSinceStartup;
                    
                    //COMMENT : 피버 상태 OFF
                    GlobalManager.Instance.IsFever = false;
                    EventManager.Instance.TriggerEvent(GameProgressEventType.FEVER_UPDATED, this);
                }
                break;
            default:
                break;
        }
    }
    
    private void SetFeverFill(float fillAmt)
    {
        // 피버 게이지 UI에 값 반영 및 색상 변경(가득 차면 빨간색, 아니면 초록색)
        feverFillImage.fillAmount    = fillAmt;
        feverFillImage.color         = fillAmt >= 1f ? Color.red : Color.green;
    }
    
    /// <summary>
    /// 게이지 증가 로직
    /// </summary>
    public void IncreaseFever()
    {
        // 피버가 아닐 때만 게이지 증가
        if (ActionType == FeverActionType.NONE)
        {
            currentFeverValue += currentIncreaseAmount; // 게이지 증가
            currentFeverValue = Mathf.Clamp(currentFeverValue, 0, maxFeverValue); // 최대값 제한
 
            // 게이지 증가 시 가속도 적용(최대 증가량 제한)
            currentIncreaseAmount = Mathf.Min(currentIncreaseAmount * accelerationFactor, maxIncreaseRate);
        }
    }
    
    /// <summary>
    /// 게이지 감소 로직
    /// </summary>
    private void DecreaseFever()
    {
        if (currentFeverValue > 0)
        {
            currentFeverValue -= currentDecreaseRate * Time.deltaTime; // 시간에 따라 게이지 감소
            currentFeverValue = Mathf.Clamp(currentFeverValue, 0, maxFeverValue); // 최소값 제한
            
            // 게이지 감소 시 가속도 적용(최대 감소량 제한)
            currentDecreaseRate = Mathf.Min(currentDecreaseRate * accelerationFactor, maxDecreaseRate);
        }
        else
        {
            // 게이지가 0일 때 감소 속도 초기화
            currentDecreaseRate = initialDecreaseRate;
        }
    }
}
