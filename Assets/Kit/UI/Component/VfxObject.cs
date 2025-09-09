using UnityEngine;
using GameLogic.Core;

/// <summary>
/// VFX Object 관련 처리 컴포넌트
/// 파티클이 풀링일 경우 해당 컴포턴트를 사용해서 재사용 처리
/// </summary>
public class VfxObject : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake() 
    {
        particle = GetComponent<ParticleSystem>();
    }
    
    private void Update()
    {
        if (particle == null)
            return;

        //파티클이 끝나면 오브젝트를 반환.
        if (particle.isStopped)
            this.gameObject.Release();
    }
}
