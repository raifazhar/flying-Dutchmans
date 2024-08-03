using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteInEditMode]
public class GameSoundAssets : MonoBehaviour
{
    public static GameSoundAssets Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }



    public SoundAudioClip[] AudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        [HideInInspector] public string name;
        public AudioClip[] sounds;

        [Header("Settings")]
        public bool hasCoolDown;
        public float CoolDownTime;
       
    }

#if UNITY_EDITOR

    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundManager.Sound));
        SoundManager.soundTimerDictionary = new Dictionary<string,SoundManager.SoundTime>();
        Array.Resize(ref AudioClips, names.Length);
        for(int i = 0; i < names.Length; i++)
        {
            Debug.Log(names[i]);
            AudioClips[i].name = names[i];
            if (AudioClips[i].hasCoolDown)
            {
                SoundManager.soundTimerDictionary[names[i]] = new SoundManager.SoundTime { currentTime = 0, maxTime = AudioClips[i].CoolDownTime };
            }
        }
        Debug.Log(SoundManager.soundTimerDictionary);
    }
#endif
}
