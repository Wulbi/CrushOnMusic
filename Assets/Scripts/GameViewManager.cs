using System;
using UnityEngine;
using TMPro;
using BigNumber;
using GameLogic.Enum;
using GameLogic.Manager;
using UnityEngine.EventSystems;

public class GameViewManager : MonoBehaviour
{
    public Animator kiwiAnim;

    /// <summary>
    /// 키위 본체 이미지
    /// </summary>
    public SpriteRenderer kiwiImage;
    
    //변경할 이미지들
    public Sprite defaultKiwi;
    public Sprite goldKiwi;
    
    /// <summary>
    /// 보조 오브젝트 복제할 프리팹
    /// </summary>
    public GameObject assistView01Prefab;
    /// <summary>
    /// 보조 오브젝트 생성할 위치
    /// </summary>
    public Transform  assistViewRoot;
    
    /// <summary>
    /// 키위 오브젝트 기준(원형)에 보조 오브젝트를 얼마나 보여줄 것인지?
    /// </summary>
    public int    assistViewPerCircle;
    /// <summary>
    /// 키위 오브젝트에서 기준점에서부터 얼마나 떨어져서 생성할 것인지?
    /// </summary>
    public float  distanceFromCenter;
    /// <summary>
    /// 회전 속도
    /// </summary>
    public float  rotationSpeed;

    /// <summary>
    /// 효과 보여주는 위치 (부모 위치)
    /// </summary>
    public RectTransform effectRoot;
    
    private void OnEnable()
    {
        //ViewManager 오브젝트가 활성화
        EventManager.Instance.AddListener<Vector3>(GameProgressEventType.TOUCH_BEGIN, OnTouchBegin);
        EventManager.Instance.AddListener<AssistContainer>(GameProgressEventType.ASSIST_VIEW_UPGRADE, OnAssistViewUpgrade);
        EventManager.Instance.AddListener<FeverBar>(GameProgressEventType.FEVER_UPDATED, OnFeverUpdated);
        EventManager.Instance.AddListener(GameProgressEventType.GAME_STARTED, OnGameStarted);
    }

    private void OnDisable()
    {
        if (EventManager.HasInstance == false)
            return;
        
        //ViewManager 오브젝트가 비활성화
        EventManager.Instance.RemoveListener<Vector3>(GameProgressEventType.TOUCH_BEGIN, OnTouchBegin);
        EventManager.Instance.RemoveListener<AssistContainer>(GameProgressEventType.ASSIST_VIEW_UPGRADE, OnAssistViewUpgrade);
        EventManager.Instance.RemoveListener<FeverBar>(GameProgressEventType.FEVER_UPDATED, OnFeverUpdated);
        EventManager.Instance.RemoveListener(GameProgressEventType.GAME_STARTED, OnGameStarted);
    }

    private void OnGameStarted()
    {
        //데이터 다 처리되고 -> 바로 이벤트 쏘면 된다
        for (int i = 0; i < GlobalManager.Instance.assistClickLevelList[0].level; i++)
        {
            //AddAssistView();
        }
    }
    private void OnFeverUpdated(FeverBar feverBar)
    {
        if (feverBar.ActionType == FeverBar.FeverActionType.ACTIVE)
        {
            //kiwiImage.sprite = goldKiwi;
        }
        else
        {
            //kiwiImage.sprite = defaultKiwi;
        }
    }
    private void OnTouchBegin(Vector3 pos)
    {
        AddKiwi(GlobalManager.Instance.GetTouchAmount());
        //kiwiAnim.Play("Touch",0,0f);

        SoundManager.Instance.PlaySfx(CommonSounds.GetClip(SfxType.TOUCH_BEGIN));
        
        Vector2 localPoint = VectorUtils.ConvertToLocalPos(effectRoot, pos, Camera.main);
        EffectManager.Instance.SpawnFloatingText(GlobalManager.Instance.GetTouchAmount().ToCustomString(), localPoint, effectRoot);

        Vector2 worldPoint = VectorUtils.ConvertToWorldPos(effectRoot, pos, Camera.main);
        EffectManager.Instance.SpawnTouchEffect(worldPoint, Quaternion.Euler(-90f, 0f, 0f));
    }
    private void OnAssistViewUpgrade(AssistContainer container)
    {
        /*
        if (container.order == 0)
            SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.DRUM));
        if (container.order == 1)
            SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.BASS));
        if (container.order == 2)
            SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.GUITAR1));
        if (container.order == 4)
            SoundManager.Instance.PlayMusic(CommonSounds.GetClip(MusicType.KEYBOARD));
        */

    }
    private void AddKiwi(BigDouble amt)
    {
        GlobalManager.Instance.kiwiAmount = GlobalManager.Instance.kiwiAmount + amt;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }
    private void Update()
    {
        RotateAssistView();
    }
    
    private void RotateAssistView()
    {
        // assistViewRoot 게임 오브젝트를 Z축(forward) 방향으로 회전시킵니다.
        // Time.deltaTime을 곱해 프레임 속도에 관계없이 일정한 속도로 회전하게 합니다.
        // rotationSpeed는 회전 속도를 제어하는 변수입니다.
        assistViewRoot.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed);
    }
    private void AddAssistView()
    {
        // 최대 생성 가능한 오브젝트 수를 확인합니다.
        // 현재 자식 개수가 원에 배치할 최대 개수에 도달했다면 더 이상 생성하지 않습니다.
        if (assistViewRoot.childCount >= assistViewPerCircle)
            return;

        // 오브젝트들 사이의 각도 간격을 계산합니다.
        // 360도를 전체 오브젝트 개수로 나누어 균등한 간격을 얻습니다.
        float angleStep             = 360f / assistViewPerCircle;
        
        // 현재 생성할 오브젝트의 각도를 계산합니다.
        // 자식 개수에 각도 간격을 곱하여 위치할 각도를 결정합니다.
        float targetAngle           = angleStep * assistViewRoot.childCount;
        
        // 라디안 각도로 변환합니다. (Unity의 삼각함수는 라디안을 사용합니다)
        // Deg2Rad는 약 0.01745329로, 각도를 라디안으로 변환하는 상수입니다.
        float targetRadianAngle     = Mathf.Deg2Rad * targetAngle;

        // 오브젝트가 위치할 로컬 좌표를 초기화합니다.
        Vector3 targetLocalPosition = Vector3.zero;
        
        // 삼각함수를 사용하여 원 위의 x 좌표를 계산합니다.
        // 코사인 함수는 각도의 x 좌표 성분을 계산합니다.
        targetLocalPosition.x = distanceFromCenter * Mathf.Cos(targetRadianAngle);
        
        // 삼각함수를 사용하여 원 위의 y 좌표를 계산합니다.
        // 사인 함수는 각도의 y 좌표 성분을 계산합니다.
        targetLocalPosition.y = distanceFromCenter * Mathf.Sin(targetRadianAngle);

        // 계산된 로컬 좌표를 월드 좌표로 변환합니다.
        // 부모 오브젝트(assistViewRoot)의 위치와 회전을 고려하여 실제 월드 상의 위치를 얻습니다.
        Vector3 targetWorldPosition = assistViewRoot.TransformPoint(targetLocalPosition);

        // 2D 환경에서 오브젝트가 중심을 바라보게 하기 위한 방향 벡터를 계산합니다.
        // 중심점(assistViewRoot의 위치)에서 오브젝트의 위치를 빼서 방향을 얻습니다.
        // Vector2로 캐스팅하여 2D 평면에서의 방향만 고려합니다.
        Vector2 directionToCenter = (Vector2)assistViewRoot.position - (Vector2)targetWorldPosition;
        
        // 방향 벡터의 각도를 계산합니다.
        // Atan2 함수는 x와 y 성분으로부터 각도를 계산하며, 결과는 라디안이므로 도(degree)로 변환합니다.
        float angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;


        // 2D에서는 Z축을 중심으로 회전만 필요합니다.
        // 계산된 각도에서 90도를 빼는 이유는 오브젝트의 '위' 방향이 중심을 향하게 하기 위함입니다.
        // 스프라이트의 기본 방향이 위쪽을 향하고 있다면, 중심을 바라보려면 90도 조정이 필요합니다.
        Quaternion targetRotation = Quaternion.Euler(0, 0, angleToCenter - 90);
        
        // 최종적으로 계산된 위치와 회전값으로 프리팹을 인스턴스화합니다.
        // 생성된 오브젝트는 assistViewRoot의 자식으로 설정됩니다.
        Instantiate(assistView01Prefab, targetWorldPosition, targetRotation, assistViewRoot);        
    }
    
}
