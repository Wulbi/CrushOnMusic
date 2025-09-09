using UnityEngine;
using GameLogic.Enum;

public class Musician : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("자동으로 설정할 루프 타입")]
    public LoopClipType[] loopClipTypes;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void SetData(int order, int level, int grade)
    {
        if (order >= 0 && order < loopClipTypes.Length)
        {
            LoopClipType type = loopClipTypes[order];
            AudioClip clip = CommonSounds.GetClip(type, grade);

            if (clip != null && level > 0)
            {
                audioSource.clip = clip;
            }
            else
            {
                Debug.LogWarning($"[Musician] 음악 클립이 존재하지 않거나 레벨이 0 이하입니다. LoopClipType: {type}, Grade: {grade}");
            }
        }
        else
        {
            Debug.LogError($"[Musician] order 인덱스가 loopClipTypes 범위를 벗어났습니다. order: {order}, 배열 길이: {loopClipTypes.Length}");
        }
    }

    public void Play()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
            audioSource.Play();
        }
    }

    public void Mute(bool shouldMute)
    {
        if (audioSource != null)
        {
            audioSource.mute = shouldMute;
        }
    }
}