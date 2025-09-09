using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionContainer : MonoBehaviour
{
    public Image Icon;
    //컨테이너 이름 설명 텍스트
    public TMP_Text labelName;
    //컨테이너 내용 설명 텍스트
    public TMP_Text labelDesc;
    
    public Button rewardButton;

    public GameObject doneRect;
    
    private AchievementState State;
    
    private void Awake()
    {
        rewardButton.onClick.AddListener(OnClickedReward);
    }

    public void SetData(AchievementState state)
    {
        State = state;
        
        labelName.text      = state.Data.Name;
        int maxLv           = state.IsDone() ? state.GetMaxLevel() : state.Level;
        labelDesc.text      = string.Format(state.Data.Desc, state.CurrentCount, state.Data.GoalCount[maxLv]);
        Icon.sprite         = state.Data.Icon;
        //완료 패널을 활성화 (조건 : 업적을 모두 달성했을 경우)
        doneRect.SetActive(state.IsDone());
        
        rewardButton.interactable = !state.IsDone() && state.IsReward;
    }

    public void OnClickedReward()
    {
        if (State == null)
            return;

        // Background sprite가 null이 아닌 경우에만 GlobalManager 배경 교체
        if (State.Data.Background != null)
        {
            // GlobalManager의 background가 비어 있다면 태그로 찾기
            if (GlobalManager.Instance.background == null)
            {
                GlobalManager.Instance.background = GameObject.FindWithTag("Background");
            }

            if (GlobalManager.Instance.background != null)
            {
                var spriteRenderer = GlobalManager.Instance.background.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = State.Data.Background;
                }
                else
                {
                    Debug.LogWarning("[MissionContainer] Background 오브젝트에 SpriteRenderer가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("[MissionContainer] 태그 'Background'로 오브젝트를 찾을 수 없습니다.");
            }
        }

        State.Reward();
    }
}
