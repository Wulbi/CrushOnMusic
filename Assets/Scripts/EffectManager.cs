using UnityEngine;
using GameLogic.Core;
using GameLogic.Manager;

public class EffectManager : SingletonBehaviour<EffectManager>
{
    
    public void SpawnFloatingText(string msg, Vector2 spawnPos, Transform spawnRect)
    {
        Transform trans = PoolManager.Instance.GetFromPool("FLOATING_TEXT", spawnRect).transform;
        trans.position         = Vector3.zero; //new Vector3 0,0,0
        trans.localPosition    = Vector3.zero;

        FloatingText floatingText = trans.GetComponent<FloatingText>();
        floatingText.Init(spawnPos);
        floatingText.SetText(msg);
        
        //오브젝트 풀링 -> 활성화.
        trans.gameObject.SetActive(true);
    }

    public void SpawnTouchEffect(Vector3 spawnPos, Quaternion spawnRotation)
    {
        Transform trans = PoolManager.Instance.GetFromPool("TOUCH_EFFECT",spawnPos, spawnRotation).transform;
        trans.gameObject.SetActive(true);
    }
}
