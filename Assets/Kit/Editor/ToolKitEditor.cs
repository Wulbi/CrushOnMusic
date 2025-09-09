using UnityEngine;
using UnityEditor;

namespace GameLogic
{
    public class ToolKitEditor : Editor
    {
        [MenuItem("Tools/SKKU/Kit/ClearUserData")]
        public static void ClearUserData()
        { 
            //[주의] 모든 저장된 부분을 삭제한다.
            bool decision = EditorUtility.DisplayDialog("WARNING", "저장된 데이터를 모두 삭제하시겠습니까?", "네", "아니요");

            if (decision)
            {
                UserSettings.DeleteAll();
                UnityEngine.Debug.Log($"<color=red><b>저장 데이터 모두 삭제했습니다 :)</b></color>");
            }
        }
    }
}

