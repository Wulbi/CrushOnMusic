using UnityEngine;
using TMPro;
using BigNumber;
using UnityEngine.EventSystems;

public class TouchManager : SingletonBehaviour<TouchManager>
{
    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            AchievementManager.Instance.CheckAchievement(AchievementType.CLICK_COUNT, 1);
            EventManager.Instance.TriggerEvent(GameProgressEventType.TOUCH_BEGIN, Input.mousePosition);
        }
    }
}
