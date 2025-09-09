using System;
using System.Collections.Generic;
using BigNumber;
using DG.Tweening;
using GameLogic.UI.Custom;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//using System.Collections.Generic;

public class MainPanel : BasePanel
{
    public TMP_Text labelKiwi;
    public TMP_Text labelKps;
    
    public GameObject mainPref;
    public GameObject assistPref;
    
    public RectTransform containerRoot;

    public GameObject musicPref;
    public RectTransform musicRoot;
    
    public FeverBar fever;
    
    public List<GameObject> containerList = new List<GameObject>();
    public List<GameObject> musicList = new List<GameObject>();
    
    public Tabs tabs;
    
    public TabType currentTab = TabType.NONE;

    //메인 탭 눌렀을 때 메인을 구성하는 프리팹 최상위 부모 오브젝트.
    public RectTransform mainRoot;
    //상점 탭 눌렀을 때 상점을 구성하는 프리팹 최상위 부모 오브젝트.
    public RectTransform shopRoot;

    public SimpleButton buttonReset;
    public SimpleButton buttonMission;
    
    public GameObject missionNoti;

    public bool wasFever = false;
    
    private float musicLoopTimer = 0f;
    private float musicLoopInterval = 12f;
    private void Awake()
    {
        buttonReset.OnClick = OnClickedReset;
        buttonMission.OnClick = OnClickedMission;
        
        missionNoti.SetActive(false);
    }

    private void OnClickedReset()
    {
        DOTween.KillAll(); 
        
        //데이터를 초기화.
        UserSettings.DeleteAll();
        
        //씬을 한번 다시 불러올거야
        SceneManager.LoadScene(0);
    }
    
    private void OnClickedMission()
    {
        UIManager.Instance.PushPanel(UIPanelType.MISSION_PANEL);
    }
    private void OnEnable()
    {
        EventManager.Instance.AddListener<Vector3>(GameProgressEventType.TOUCH_BEGIN, OnTouchBegin);
        EventManager.Instance.AddListener<AssistContainer>(GameProgressEventType.ASSIST_VIEW_UPGRADE, OnAssistViewUpgrade);
        EventManager.Instance.AddListener<FeverBar>(GameProgressEventType.FEVER_UPDATED, OnFeverUpdated);
        EventManager.Instance.AddListener<Musician>(GameProgressEventType.MUSIC_BEGIN, OnMusicBegin);
        EventManager.Instance.AddListener<Musician>(GameProgressEventType.MUSIC_END, OnMusicEnd);
        EventManager.Instance.AddListener<AchievementState>(GameProgressEventType.ACHIEVEMENT_UPDATED, OnAchievementUpdated);

        tabs.OnTabSwitched += OnTabSwitched;
    }

    private void OnDisable()
    {
        if (EventManager.HasInstance == false)
            return;
        
        EventManager.Instance.RemoveListener<Vector3>(GameProgressEventType.TOUCH_BEGIN, OnTouchBegin);
        EventManager.Instance.RemoveListener<AssistContainer>(GameProgressEventType.ASSIST_VIEW_UPGRADE, OnAssistViewUpgrade);
        EventManager.Instance.RemoveListener<FeverBar>(GameProgressEventType.FEVER_UPDATED, OnFeverUpdated);
        EventManager.Instance.RemoveListener<Musician>(GameProgressEventType.MUSIC_BEGIN, OnMusicBegin);
        EventManager.Instance.RemoveListener<Musician>(GameProgressEventType.MUSIC_END, OnMusicEnd);
        EventManager.Instance.RemoveListener<AchievementState>(GameProgressEventType.ACHIEVEMENT_UPDATED, OnAchievementUpdated);

        tabs.OnTabSwitched -= OnTabSwitched;
    }

    private void OnAchievementUpdated(AchievementState state)
    {
        //진행도가 바뀌거나, 보상을 받았을 때
        missionNoti.SetActive(AchievementManager.Instance.GetRewardStatus());
    }

    private void OnMusicEnd(Musician musician)
    {
        if (musician != null && musician.audioSource != null)
        {
            if (musician.audioSource.isPlaying)
            {
                musician.audioSource.Stop();
            }
        }
    }
    
    private void OnTabSwitched(TabType tabType)
    {
        if (currentTab == tabType)
            return;

        currentTab = tabType;
        SetData(tabType);
    }

    private void SetData(TabType tabType)
    {
        mainRoot.gameObject.SetActive(tabType == TabType.CONTENTS_VIEW_MAIN);
        shopRoot.gameObject.SetActive(tabType == TabType.CONTENTS_VIEW_SHOP);
        
        switch (tabType)
        {
            case TabType.CONTENTS_VIEW_MAIN:
                {
                    //메인 탭으로 들어오는 경우.   
                    SetContainers();
                    SetKps();
                }
                break;
            case TabType.CONTENTS_VIEW_SHOP:
                {
                    //상점 탭으로 들어오는 경우.
                }
                break;
        }
    }
    
    
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);
        
        fever.Init();
        
        this.gameObject.SetActive(true);
        
        tabs.Prepare(new List<TabType>() { TabType.CONTENTS_VIEW_MAIN , TabType.CONTENTS_VIEW_SHOP }
            , TabType.CONTENTS_VIEW_MAIN);
    }
    
    private void OnFeverUpdated(FeverBar feverBar)
    {
        //메인 컨테이너, 보조 컨테이너들
        MainContainer mainContainer = containerList
            .Find(x => x.GetComponent<MainContainer>() != null).GetComponent<MainContainer>();
        
        if (mainContainer != null)
            mainContainer.SetData();
    }
    private void OnTouchBegin(Vector3 touchPos)
    {
        fever.IncreaseFever();

        // 클릭 시 음악 시작
        if (!GlobalManager.Instance.IsFever)
        {
            foreach (var musicObj in musicList)
            {
                Musician musician = musicObj.GetComponent<Musician>();
                if (musician != null)
                {
                    EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_END, musician);
                    EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_BEGIN, musician);
                }
            }
        }
    }

    private void OnAssistViewUpgrade(AssistContainer container)
    {
        SetKps();
    }

    private void OnMusicBegin(Musician musician)
    {
        if (musician != null && musician.audioSource != null)
        {
            if (!musician.audioSource.isPlaying)
            {
                musician.audioSource.loop = false;
                musician.audioSource.Play();
            }
        }
    }

    private void SetKps()
    {
        BigDouble kps = GlobalManager.Instance.GetKPS();
        if (kps <= 0)
        {
            labelKps.text = "- like per sec";
        }
        else
        {
            labelKps.text = $"{kps.ToCustomString()} likes per sec";
        }
    }
    
    
    /// <summary>
    /// 컨테이너를 만드는 함수. (동적 생성)
    /// </summary>
    public void SetContainers()
    {
        ClearContainersExceptMusicians(); 

        AddMainContainer();

        int assistCount = GetActiveAssistCount();
        for (int i = 0; i < assistCount; i++)
        {
            var assistData = GlobalManager.Instance.assistClickLevelList[i];

            GameObject assistObj = Instantiate(assistPref, containerRoot);
            AssistContainer assist = assistObj.GetComponent<AssistContainer>();
            assist.order = i;
            assist.level = assistData.level;
            assist.grade = assistData.grade;
            assist.mainPanel = this;
            assist.transform.localScale = Vector3.one;

            Musician musician;
            if (i < musicList.Count)
            {
                // 기존 오브젝트 재사용
                musician = musicList[i].GetComponent<Musician>();
            }
            else
            {
                // 새로 생성
                GameObject musicObj = Instantiate(musicPref, musicRoot);
                musician = musicObj.GetComponent<Musician>();
                musicList.Add(musicObj);
            }

            musician.SetData(i, assistData.level, assistData.grade);
            assist.linkedMusician = musician;
            assist.SetMuteState(assistData.isMuted);

            containerList.Add(assistObj);
        }
    }

    private void ClearContainersExceptMusicians()
    {
        foreach (var obj in containerList)
            Destroy(obj);

        containerList.Clear();
    }
    
    
    private void ClearContainers()
    {
        foreach (var obj in containerList)
            Destroy(obj);
        foreach (var music in musicList)
            Destroy(music);
    
        containerList.Clear();
        musicList.Clear();
    }
    
    private void AddMainContainer()
    {
        GameObject mainObj = Instantiate(mainPref, containerRoot);
        mainObj.transform.localScale = Vector3.one;
        containerList.Add(mainObj);
    }

    private int GetActiveAssistCount()
    {
        int count = 0;
        foreach (var data in GlobalManager.Instance.assistClickLevelList)
        {
            if (data.level > 0) count++;
        }

        return Mathf.Min(count + 1, GlobalManager.Instance.assistClickLevelList.Count);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (labelKiwi != null && GlobalManager.HasInstance)
        {
            labelKiwi.text = "Likes : " + GlobalManager.Instance.kiwiAmount.ToCustomString();
        }
        bool isFever = GlobalManager.Instance.IsFever;

        // Fever 진입 시점에만 재생 리셋
        if (isFever && !wasFever)
        {
            foreach (var musicObj in musicList)
            {
                Musician musician = musicObj.GetComponent<Musician>();
                if (musician != null)
                {
                    EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_END, musician);
                    EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_BEGIN, musician);
                }
            }

            musicLoopTimer = 0f; // 루프 타이머 초기화
        }
        
        // Fever가 끝난 순간: 음악 정지
        if (!isFever && wasFever)
        {
            foreach (var musicObj in musicList)
            {
                Musician musician = musicObj.GetComponent<Musician>();
                if (musician != null)
                {
                    EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_END, musician);
                }
            }
        }


        // Fever 유지 중일 때만 타이머 작동
        if (isFever)
        {
            musicLoopTimer += Time.deltaTime;
            if (musicLoopTimer >= musicLoopInterval)
            {
                musicLoopTimer = 0f;

                foreach (var musicObj in musicList)
                {
                    Musician musician = musicObj.GetComponent<Musician>();
                    if (musician != null && !musician.audioSource.isPlaying)
                    {
                        EventManager.Instance.TriggerEvent(GameProgressEventType.MUSIC_BEGIN, musician);
                    }
                }
            }
        }

        // 이전 프레임의 상태 저장
        wasFever = isFever;
        
    }

    public override UIPanelType TypeOfPanel => UIPanelType.MAIN_PANEL;
    
    public override void OnClose()
    {
        base.OnClose();

        this.gameObject.SetActive(false);
    }
}
