using System;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Enum;

[Serializable]
public struct MusicData { public AudioClip clip; }

[Serializable]
public struct SfxData { public AudioClip clip; }

[Serializable]
public class LoopClipEntry
{
    public LoopClipType type;
    public List<AudioClip> clips;
}

[CreateAssetMenu(fileName = "CommonSoundsAsset", menuName = "Kit/Sound/CommonSounds", order = 1)]
public class CommonSounds : ScriptableObject
{
    [Header("[배경음]")]
    [SerializeField] private MusicTypeToData _musicTypeToData;

    [Header("[효과음]")]
    [SerializeField] private SfxTypeToData _sfxTypeToData;

    [Header("[루프 악기]")]
    [SerializeField] private List<LoopClipEntry> _loopEntries = new List<LoopClipEntry>();

    private static CommonSounds _commonSoundsInstance;

    private AudioClip FindMusicClip(MusicType type)
    {
        MusicData musicData;
        return _musicTypeToData.TryGetValue(type, out musicData) ? musicData.clip : null;
    }

    private AudioClip FindSfxClip(SfxType type)
    {
        SfxData sfxData;
        return _sfxTypeToData.TryGetValue(type, out sfxData) ? sfxData.clip : null;
    }

    private AudioClip FindLoopClip(LoopClipType type, int grade)
    {
        var entry = _loopEntries.Find(e => e.type == type);
        if (entry != null && entry.clips != null && entry.clips.Count > 0)
        {
            int safeGrade = Mathf.Clamp(grade, 0, entry.clips.Count - 1);
            return entry.clips[safeGrade];
        }
        return null;
    }

    public static AudioClip GetClip(MusicType type)
    {
        if (_commonSoundsInstance == null)
            _commonSoundsInstance = Resources.Load<CommonSounds>("CommonSoundsAsset");

        return _commonSoundsInstance.FindMusicClip(type);
    }

    public static AudioClip GetClip(SfxType type)
    {
        if (_commonSoundsInstance == null)
            _commonSoundsInstance = Resources.Load<CommonSounds>("CommonSoundsAsset");

        return _commonSoundsInstance.FindSfxClip(type);
    }

    public static AudioClip GetClip(LoopClipType type, int grade)
    {
        if (_commonSoundsInstance == null)
            _commonSoundsInstance = Resources.Load<CommonSounds>("CommonSoundsAsset");

        return _commonSoundsInstance.FindLoopClip(type, grade);
    }

    [Serializable]
    public class MusicTypeToData : SerializableDictionary<MusicType, MusicData> { }

    [Serializable]
    public class SfxTypeToData : SerializableDictionary<SfxType, SfxData> { }
}
