using UnityEngine;

namespace GameLogic.UI.Custom
{
    public interface ITabButton
    {
        TabType TargetTabType { get; }

        RectTransform thisRectTransform { get; }

        void Init(Tabs tabs, TabType tabType);

        void OnTabSwitched(TabType tabType);

        bool IsEnabled { set; }

        bool IsSelected { set; }

        bool IsLockEnabled { set; }                 //잠김 처리 세팅

        bool IsLockObjectActive { set; }            //잠김 오브젝트만 세팅

        string LockText { set; }

        bool IsNotiEnabled { set; }                 //알림 처리 세팅

        string NotiText { set; }
    }
}
