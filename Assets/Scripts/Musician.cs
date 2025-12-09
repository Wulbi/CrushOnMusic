using UnityEngine;
using GameLogic.Enum;
using UnityEngine.UI;

public class Musician : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("자동으로 설정할 루프 타입")]
    public LoopClipType[] loopClipTypes;

    public Image MusicianImage;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
    }

    public void SetData(int order, int level, int grade)
    {
        AudioClip clip = null;
        
        // Try to get clip from UpgradeDB first
        if (DatabaseManager.HasInstance && DatabaseManager.Instance.upgradeDB != null)
        {
            var assistDataList = DatabaseManager.Instance.upgradeDB.assistDataList;
            if (assistDataList != null && order >= 0 && order < assistDataList.Count)
            {
                var assistData = assistDataList[order];
                if (assistData.loopClips != null && assistData.loopClips.Count > 0)
                {
                    // Use grade to select clip, or first clip if grade is out of range
                    int clipIndex = Mathf.Clamp(grade, 0, assistData.loopClips.Count - 1);
                    clip = assistData.loopClips[clipIndex];
                }
            }
        }
        
        // Fallback to loopClipTypes array if UpgradeDB doesn't have clips
        if (clip == null && loopClipTypes != null && order >= 0 && order < loopClipTypes.Length)
        {
            Debug.LogWarning($"[Musician] UpgradeDB에 loopClips가 없습니다. order: {order}, loopClipTypes 배열을 사용합니다.");
            // Note: Can't get clip from CommonSounds anymore, so just log warning
        }

        if (clip != null && level > 0)
        {
            audioSource.clip = clip;
        }
        else
        {
            if (level <= 0)
            {
                Debug.LogWarning($"[Musician] 레벨이 0 이하입니다. order: {order}, Level: {level}");
            }
            else
            {
                Debug.LogWarning($"[Musician] 음악 클립이 존재하지 않습니다. order: {order}, UpgradeDB에 loopClips를 설정해주세요.");
            }
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