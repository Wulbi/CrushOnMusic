using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//탭 버튼 상위 하이라키에 있어야한다.
namespace GameLogic.UI.Custom
{
    public class Tabs : MonoBehaviour
    {
        private ITabButton[] _tabButtons;
        private bool isEnabled = false;

        private Action<TabType> onTabSwitched;

        /// <summary>
        /// 탭 클릭을 누를 때마다 호출하는 이벤트
        /// </summary>
        public event Action<TabType> OnTabSwitched
        {
            add { onTabSwitched += value; }
            remove { onTabSwitched -= value; }
        }

        public TabType CurrentTab { get; private set; }
        public void Prepare(List<TabType> tabs, TabType enabledTab = TabType.NONE, List<TabType> hiddenTabs = null)
        {
            this._tabButtons = this.gameObject.GetComponentsInChildren<ITabButton>(true).Where(x => x.thisRectTransform.gameObject.activeSelf == true).ToArray();

            if (this._tabButtons.Length != tabs.Count)
            {
                Debug.LogError((object)("Layout has incorrect amount of buttons, button count should be: " + (object)tabs.Count));
            }
            else
            {
                isEnabled = true;
                for (int index = 0; index < tabs.Count; ++index)
                {
                    ITabButton tabButton = this._tabButtons[index];
                    TabType tabType = tabs[index];

                    tabButton.Init(this, tabType);
                    tabButton.IsEnabled = true;
                    tabButton.IsSelected = enabledTab == tabType;
                }
            }
        }
        public void SwitchTab(TabType tabType)
        {
            if (!isEnabled)
                return;

            if (_tabButtons == null)
                return;

            this.CurrentTab = tabType;
            
            foreach (ITabButton btn in this._tabButtons)
                btn.OnTabSwitched(tabType);

            this.onTabSwitched.Fire<TabType>(tabType);
        }
        public void SetEnabled(bool enabled) => isEnabled = enabled;
        public RectTransform GetTabRectTransform(TabType tabType) => _tabButtons.First<ITabButton>(x => x.TargetTabType == tabType).thisRectTransform;
        public ITabButton GetTabButton(TabType tabType) => _tabButtons.First<ITabButton>(x => x.TargetTabType == tabType);
    }
}

