using System;
using UnityEngine;
using TMPro;
using GameLogic.Core;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    /// <summary>
    /// 연출할 텍스트
    /// </summary>
    public TMP_Text               text;

    public float                  riseSpeed       = 0.55f;      // 올라가는 속도
    public float                  animationTime   = 0.55f;      // 연출 시간
    public float                  targetY         = -200f;      // 목표 Y 값
    
    private float               timer;              //연출 시간 계산용 변수
    private RectTransform       root      = null;   //움직일 위치 컴포넌트
    //private bool                started;            //연출 시작 알림 체크 변수
    private float               startY    = 0f;     //최초 시작 지점 설정

    private Sequence seq;
    
    private void OnEnable()
    {
        //started     = false;
        root        = this.transform.GetComponent<RectTransform>();

        transform.localScale = Vector3.one;
    }

    // private void Update()
    // {
    //     if (!started)
    //         return;
    //     
    //     HandleText();
    // }

    /// <summary>
    /// 재 사용시 다시 값을 세팅하는 함수
    /// </summary>
    /// <param name="spawnPos">생성 위치 값</param>
    public void Init(Vector3 spawnPos)
    {
        transform.localScale = Vector3.one;
        root.anchoredPosition = spawnPos;
        
        Color color = text.color;
        color.a = 1f;
        text.color = color;

        timer   = 0;
        //started = true;
        startY  = root.anchoredPosition.y;
        
        //
        seq?.Kill();
        seq = DOTween.Sequence();
        
        seq.Append(root.DOAnchorPosY(spawnPos.y + targetY, animationTime).SetEase(Ease.OutQuad));
        seq.Join(text.DOFade(0f, animationTime).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            gameObject.Release();
        });

        seq.Play();
    }

    
    public void SetText(string msg)
    {
        text.text = msg;
    }
    private void HandleText()
    {
        timer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(timer / animationTime);
        
        if (normalizedTime < 1)
        {
            Color color = text.color;
            color.a = 1f - normalizedTime;
            text.color = color;
            
            float newY = Mathf.Lerp(startY, startY + targetY, normalizedTime);
            
            // 현재 위치 업데이트
            Vector2 newPosition = root.anchoredPosition;
            newPosition.y = newY;
            root.anchoredPosition = newPosition;
        }
        else
        {
            // 애니메이션 완료 후 처리 (풀링 다시 보관)
            gameObject.Release();
        }
    }
}
