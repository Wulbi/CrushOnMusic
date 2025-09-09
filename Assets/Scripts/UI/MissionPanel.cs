 using System;
 using System.Collections.Generic;
using UnityEngine;

public class MissionPanel : BasePanel
{
    /// <summary>
    /// 컨테이너들 생성할 위치
    /// </summary>
    public RectTransform    containerRoot;
    
    /// <summary>
    /// 미션 컨테이너 복제할 프리팹 본체
    /// </summary>
    public MissionContainer  missionContainer;

    /// <summary>
    /// 컨테이너들 관리하는 리스트
    /// </summary>
    public List<GameObject> containers = new List<GameObject>();
    
    public override UIPanelType TypeOfPanel => UIPanelType.MISSION_PANEL;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<AchievementState>(GameProgressEventType.ACHIEVEMENT_UPDATED
            , OnAchievementUpdated);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<AchievementState>(GameProgressEventType.ACHIEVEMENT_UPDATED
            , OnAchievementUpdated);
    }

    private void OnAchievementUpdated(AchievementState state)
    {
        SetData();
    }
    /// <summary>
    /// 패널 활성화 설정
    /// </summary>
    /// <param name="datas"></param>
    public override void OnEnter(params object[] datas)
    {
        base.OnEnter(datas);

        SetData();
        
        // 패널에 접근하면 오브젝트를 활성화 해준다.
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 패널 비활성화 설정
    /// </summary>
    public override void OnClose()
    {
        base.OnClose();
        
        this.gameObject.SetActive(false);
    }

    public void OnClickedClose()
    {
        OnClose();
    }
    
    public void SetData()
    {
        //기존 컨테이너 다 지우기
        for (int i = 0; i < containers.Count; i++)
            Destroy(containers[i]);    
        
        // 컨테이너 리스트 초기화
        containers.Clear();
       
        // 미션 컨테이너들 생성.
        for (int i = 0; i < AchievementManager.Instance.achievementList.Count; i++)
        {
            GameObject assistObj = Instantiate(missionContainer.gameObject, Vector3.zero, Quaternion.identity);
            assistObj.transform.SetParent(containerRoot);
            assistObj.transform.localScale = Vector3.one;
            assistObj.GetComponent<MissionContainer>().SetData(AchievementManager.Instance.achievementList[i]);
            containers.Add(assistObj);
        }
    }
}
