using System;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Enum;

[Serializable]
public struct MusicData { public AudioClip clip; }

[Serializable]
public struct SfxData { public AudioClip clip; }

[CreateAssetMenu(fileName = "CommonSoundsAsset", menuName = "Kit/Sound/CommonSounds", order = 1)]
public class CommonSounds : ScriptableObject
{
    [Header("[배경음]")]
    [SerializeField] private MusicTypeToData _musicTypeToData;

    [Header("[효과음]")]
    [SerializeField] private SfxTypeToData _sfxTypeToData;

    // Note: Loop clips have been moved to UpgradeDB.assistDataList[].loopClips

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

    [Serializable]
    public class MusicTypeToData : SerializableDictionary<MusicType, MusicData> { }

    [Serializable]
    public class SfxTypeToData : SerializableDictionary<SfxType, SfxData> { }
}
