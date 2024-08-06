using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteInEditMode]
public class GameSoundAssets : MonoBehaviour {
    public static GameSoundAssets Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

    }

    private void Start() {
        PopulateSoundDictionary();
    }



    public SoundAudioClip[] AudioClipsArray;

    [System.Serializable]
    public class SoundAudioClip {
        [HideInInspector] public string name;
        public sound[] audioClips;

        [Header("Settings")]
        public float masterVolume = 1f;
        public bool hasCoolDown;
        public float CoolDownTime;

        [System.Serializable]
        public struct sound {
            public string name;
            public AudioClip clip;
            [Range(-3, 3)]
            public float pitch;
            public float startTime;
            [Range(0, 1)]
            public float volume;


        }


    }

    private void PopulateSoundDictionary() {
        string[] names = Enum.GetNames(typeof(SoundManager.Sound));
        SoundManager.soundTimerDictionary = new Dictionary<string, SoundManager.SoundTime>();
        Array.Resize(ref AudioClipsArray, names.Length);
        for (int i = 0; i < names.Length; i++) {
            AudioClipsArray[i].name = names[i];
            if (AudioClipsArray[i].hasCoolDown) {
                SoundManager.soundTimerDictionary[names[i]] = new SoundManager.SoundTime { currentTime = 0, maxTime = AudioClipsArray[i].CoolDownTime };
            }
        }

    }
}
