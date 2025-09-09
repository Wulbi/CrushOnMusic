using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.Manager
{
    /// <summary>
    /// 사운드 관리자
    /// </summary>
    public class SoundManager : SingletonBehaviour<SoundManager>
    {
        /// <summary>
        /// The gameobject that the sound manager is attached to
        /// </summary>
        public GameObject Gameobject { get { return gameObject; } }

        /// <summary>
        /// When set to true, new music audios that have the same audio clip as any other music audios, will be ignored
        /// </summary>
        public bool IgnoreDuplicateMusic { get; set; }

        /// <summary>
        /// When set to true, new sound audios that have the same audio clip as any other sound audios, will be ignored
        /// </summary>
        public bool IgnoreDuplicateSfx { get; set; }

        /// <summary>
        /// Global volume
        /// </summary>
        public float GlobalVolume { get; set; }

        /// <summary>
        /// Global music volume
        /// </summary>
        public float GlobalMusicVolume { get; set; }

        /// <summary>
        /// Global sounds volume
        /// </summary>
        public float GlobalSfxVolume { get; set; }

        private Dictionary<int, SoundData> musiSoundData;
        private Dictionary<int, SoundData> sfxAudio;
        private Dictionary<int, SoundData> audioPool;
        private bool initialized = false;

        protected override void Awake()
        {
            base.Awake();

            Init();
        }
        private void Update()
        {
            UpdateAllAudio(musiSoundData);
            UpdateAllAudio(sfxAudio);
        }

        /// <summary>
        /// Initialized the sound manager
        /// </summary>
        private void Init()
        {
            if (!initialized)
            {
                musiSoundData = new Dictionary<int, SoundData>();
                sfxAudio = new Dictionary<int, SoundData>();
                audioPool = new Dictionary<int, SoundData>();

                GlobalVolume = 1;
                GlobalMusicVolume = 1;
                GlobalSfxVolume = 1;
                IgnoreDuplicateMusic = false;
                IgnoreDuplicateSfx = false;

                initialized = true;
            }
        }

        /// <summary>
        /// Event triggered when a new scene is loaded
        /// </summary>
        /// <param name="scene">The scene that is loaded</param>
        /// <param name="mode">The scene load mode</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Stop and remove all non-persistent audio
            RemoveNonPersistAudio(musiSoundData);
            RemoveNonPersistAudio(sfxAudio);
        }

        /// <summary>
        /// Retrieves the audio dictionary based on the audioType
        /// </summary>
        /// <param name="audioType">The audio type of the dictionary to return</param>
        /// <returns>An audio dictionary</returns>
        private Dictionary<int, SoundData> GetAudioTypeDictionary(SoundData.AudioType audioType)
        {
            Dictionary<int, SoundData> audioDict = new Dictionary<int, SoundData>();
            switch (audioType)
            {
                case SoundData.AudioType.Music:
                    audioDict = musiSoundData;
                    break;
                case SoundData.AudioType.Sfx:
                    audioDict = sfxAudio;
                    break;
            }

            return audioDict;
        }

        /// <summary>
        /// Retrieves the IgnoreDuplicates setting of audios of a specified audio type
        /// </summary>
        /// <param name="audioType">The audio type that the returned IgnoreDuplicates setting affects</param>
        /// <returns>An IgnoreDuplicates setting (bool)</returns>
        private bool GetAudioTypeIgnoreDuplicateSetting(SoundData.AudioType audioType)
        {
            switch (audioType)
            {
                case SoundData.AudioType.Music:
                    return IgnoreDuplicateMusic;
                case SoundData.AudioType.Sfx:
                    return IgnoreDuplicateSfx;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Updates the state of all audios of an audio dictionary
        /// </summary>
        /// <param name="audioDict">The audio dictionary to update</param>
        private void UpdateAllAudio(Dictionary<int, SoundData> audioDict)
        {
            // Go through all audios and update them
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                audio.Update();

                // Remove it if it is no longer active (playing)
                if (!audio.IsPlaying && !audio.Paused)
                {
                    Destroy(audio.AudioSource);

                    // Add it to the audio pool in case it needs to be referenced in the future
                    audioPool.Add(key, audio);
                    audio.Pooled = true;
                    audioDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// Remove all non-persistant audios from an audio dictionary
        /// </summary>
        /// <param name="audioDict">The audio dictionary whose non-persistant audios are getting removed</param>
        private void RemoveNonPersistAudio(Dictionary<int, SoundData> audioDict)
        {
            // Go through all audios and remove them if they should not persist through scenes
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                if (!audio.Persist && audio.Activated)
                {
                    Destroy(audio.AudioSource);
                    audioDict.Remove(key);
                }
            }

            // Go through all audios in the audio pool and remove them if they should not persist through scenes
            keys = new List<int>(audioPool.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioPool[key];
                if (!audio.Persist && audio.Activated)
                {
                    audioPool.Remove(key);
                }
            }
        }

        /// <summary>
        /// Restores and re-adds a pooled audio to its corresponding audio dictionary
        /// </summary>
        /// <param name="audioType">The audio type of the audio to restore</param>
        /// <param name="audioID">The ID of the audio to be restored</param>
        /// <returns>True if the audio is restored, false if the audio was not in the audio pool.</returns>
        public bool RestoreAudioFromPool(SoundData.AudioType audioType, int audioID)
        {
            if (audioPool.ContainsKey(audioID))
            {
                Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);
                audioDict.Add(audioID, audioPool[audioID]);
                audioPool.Remove(audioID);

                return true;
            }

            return false;
        }

        #region GetAudio Functions

        /// <summary>
        /// Returns the Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the Audio to be retrieved</param>
        /// <returns>Audio that has as its id the audioID, null if no such Audio is found</returns>
        public SoundData GetAudio(int audioID)
        {
            SoundData audio;

            audio = GetMusicData(audioID);
            if (audio != null)
                return audio;

            audio = GetSfxData(audioID);
            if (audio != null)
                return audio;

            return null;
        }

        /// <summary>
        /// Returns the first occurrence of Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the Audio to be retrieved</param>
        /// <returns>First occurrence of Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public SoundData GetAudio(AudioClip audioClip)
        {
            SoundData audio = GetMusicData(audioClip);
            if (audio != null)
                return audio;

            audio = GetSfxData(audioClip);
            if (audio != null)
                return audio;

            return null;
        }

        /// <summary>
        /// Returns the music Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the music Audio to be returned</param>
        /// <returns>Music Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
        public SoundData GetMusicData(int audioID)
        {
            return GetAudio(SoundData.AudioType.Music, true, audioID);
        }

        /// <summary>
        /// Returns the first occurrence of music Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the music Audio to be retrieved</param>
        /// <returns>First occurrence of music Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public SoundData GetMusicData(AudioClip audioClip)
        {
            return GetAudio(SoundData.AudioType.Music, true, audioClip);
        }

        /// <summary>
        /// Returns the sound fx Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the sound fx Audio to be returned</param>
        /// <returns>Sound fx Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
        public SoundData GetSfxData(int audioID)
        {
            return GetAudio(SoundData.AudioType.Sfx, true, audioID);
        }

        /// <summary>
        /// Returns the first occurrence of sound Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the sound Audio to be retrieved</param>
        /// <returns>First occurrence of sound Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public SoundData GetSfxData(AudioClip audioClip)
        {
            return GetAudio(SoundData.AudioType.Sfx, true, audioClip);
        }

        private SoundData GetAudio(SoundData.AudioType audioType, bool usePool, int audioID)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);

            if (audioDict.ContainsKey(audioID))
            {
                return audioDict[audioID];
            }

            if (usePool && audioPool.ContainsKey(audioID) && audioPool[audioID].Type == audioType)
            {
                return audioPool[audioID];
            }

            return null;
        }

        private SoundData GetAudio(SoundData.AudioType audioType, bool usePool, AudioClip audioClip)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);

            List<int> audioTypeKeys = new List<int>(audioDict.Keys);
            List<int> poolKeys = new List<int>(audioPool.Keys);
            List<int> keys = usePool ? audioTypeKeys.Concat(poolKeys).ToList() : audioTypeKeys;
            foreach (int key in keys)
            {
                SoundData audio = null;
                if (audioDict.ContainsKey(key))
                {
                    audio = audioDict[key];
                }
                else if (audioPool.ContainsKey(key))
                {
                    audio = audioPool[key];
                }
                if (audio == null)
                {
                    return null;
                }
                if (audio.Clip == audioClip && audio.Type == audioType)
                {
                    return audio;
                }
            }

            return null;
        }

        #endregion

        #region Prepare Function

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareMusic(AudioClip clip)
        {
            return PrepareAudio(SoundData.AudioType.Music, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareMusic(AudioClip clip, float volume)
        {
            return PrepareAudio(SoundData.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name = "persist" > Whether the audio persists in between scene changes</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PrepareAudio(SoundData.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prerpares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PrepareAudio(SoundData.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <param name="currentMusicfadeOutSeconds"> How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds</param>
        /// <param name="sourceTransform">The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PrepareAudio(SoundData.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareSound(AudioClip clip)
        {
            return PrepareAudio(SoundData.AudioType.Sfx, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareSound(AudioClip clip, float volume)
        {
            return PrepareAudio(SoundData.AudioType.Sfx, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareSound(AudioClip clip, bool loop)
        {
            return PrepareAudio(SoundData.AudioType.Sfx, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <param name="sourceTransform">The transform that is the source of the sound (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PrepareSound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PrepareAudio(SoundData.AudioType.Sfx, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        private int PrepareAudio(SoundData.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            if (clip == null)
            {
                Debug.LogError("<b>[Sound Manager]</b> Audio clip is null", clip);
            }

            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);

            if (ignoreDuplicateAudio)
            {
                SoundData duplicateAudio = GetAudio(audioType, true, clip);
                if (duplicateAudio != null)
                {
                    return duplicateAudio.AudioID;
                }
            }

            // Create the audioSource
            SoundData audio = new SoundData(audioType, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);

            // Add it to dictionary
            audioDict.Add(audio.AudioID, audio);

            return audio.AudioID;
        }

        #endregion

        #region Play Functions

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlayMusic(AudioClip clip)
        {
            return PlayAudio(SoundData.AudioType.Music, clip, 1f, true, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlayMusic(AudioClip clip, float volume)
        {
            return PlayAudio(SoundData.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name = "persist" > Whether the audio persists in between scene changes</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlayMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PlayAudio(SoundData.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInSeconds">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutSeconds"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PlayAudio(SoundData.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInSeconds">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutSeconds"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <param name="currentMusicfadeOutSeconds"> How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds</param>
        /// <param name="sourceTransform">The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PlayAudio(SoundData.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlaySfx(AudioClip clip)
        {
            return PlayAudio(SoundData.AudioType.Sfx, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlaySfx(AudioClip clip, float volume)
        {
            return PlayAudio(SoundData.AudioType.Sfx, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlaySfx(AudioClip clip, bool loop)
        {
            return PlayAudio(SoundData.AudioType.Sfx, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <param name="sourceTransform">The transform that is the source of the sound (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public int PlaySfx(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PlayAudio(SoundData.AudioType.Sfx, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        public int PlaySfx(AudioClip clip, float volume, bool loop, Transform sourceTransform, float fadeInSeconds, float fadeOutSeconds)
        {
            return PlayAudio(SoundData.AudioType.Sfx, clip, volume, loop, false, fadeInSeconds, fadeOutSeconds, -1f, sourceTransform);
        }

        private int PlayAudio(SoundData.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            int audioID = PrepareAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);

            // Stop all current music playing
            if (audioType == SoundData.AudioType.Music)
            {
                StopAllMusic(currentMusicfadeOutSeconds);
            }

            // 이 세팅이 되어있는 경우 풀링된 거 사용하도록 변경.
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);
            GetAudio(audioType, ignoreDuplicateAudio, audioID).Play(volume);

            return audioID;
        }

        #endregion

        #region Stop Functions

        /// <summary>
        /// Stop all audio playing
        /// </summary>
        public void StopAll()
        {
            StopAll(-1f);
        }

        /// <summary>
        /// Stop all audio playing
        /// </summary>
        /// <param name="musicFadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
        public void StopAll(float musicFadeOutSeconds)
        {
            StopAllMusic(musicFadeOutSeconds);
            StopAllSfx();
        }

        /// <summary>
        /// Stop all music playing
        /// </summary>
        public void StopAllMusic()
        {
            StopAllAudio(SoundData.AudioType.Music, -1f);
        }

        /// <summary>
        /// Stop all music playing
        /// </summary>
        /// <param name="fadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
        public void StopAllMusic(float fadeOutSeconds)
        {
            StopAllAudio(SoundData.AudioType.Music, fadeOutSeconds);
        }

        /// <summary>
        /// Stop all sound fx playing
        /// </summary>
        public void StopAllSfx()
        {
            StopAllAudio(SoundData.AudioType.Sfx, -1f);
        }

        public void StopAllSfx(float fadeOutSeconds)
        {
            StopAllAudio(SoundData.AudioType.Sfx, fadeOutSeconds);
        }

        public void StopSfx(AudioClip clip, float fadeOutSeconds)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(SoundData.AudioType.Sfx);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                if (audio.AudioSource.clip == clip)
                {
                    if (fadeOutSeconds > 0)
                    {
                        audio.FadeOutSeconds = fadeOutSeconds;
                    }
                    //Debug.Log("audio.stop");
                    audio.Stop();
                }
            }
        }

        private void StopAllAudio(SoundData.AudioType audioType, float fadeOutSeconds)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                if (fadeOutSeconds > 0)
                {
                    audio.FadeOutSeconds = fadeOutSeconds;
                }
                audio.Stop();
            }
        }

        #endregion

        #region Pause Functions

        /// <summary>
        /// Pause all audio playing
        /// </summary>
        public void PauseAll()
        {
            PauseAllMusic();
            PauseAllSfx();
        }

        /// <summary>
        /// Pause all music playing
        /// </summary>
        public void PauseAllMusic()
        {
            PauseAllAudio(SoundData.AudioType.Music);
        }

        /// <summary>
        /// Pause all sound fx playing
        /// </summary>
        public void PauseAllSfx()
        {
            PauseAllAudio(SoundData.AudioType.Sfx);
        }

        private void PauseAllAudio(SoundData.AudioType audioType)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                audio.Pause();
            }
        }

        #endregion

        #region Resume Functions

        /// <summary>
        /// Resume all audio playing
        /// </summary>
        public void ResumeAll()
        {
            ResumeAllMusic();
            ResumeAllSfx();
        }

        /// <summary>
        /// Resume all music playing
        /// </summary>
        public void ResumeAllMusic()
        {
            ResumeAllAudio(SoundData.AudioType.Music);
        }

        /// <summary>
        /// Resume all sound fx playing
        /// </summary>
        public void ResumeAllSfx()
        {
            ResumeAllAudio(SoundData.AudioType.Sfx);
        }

        private void ResumeAllAudio(SoundData.AudioType audioType)
        {
            Dictionary<int, SoundData> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundData audio = audioDict[key];
                audio.Resume();
            }
        }

        #endregion
    }

}
