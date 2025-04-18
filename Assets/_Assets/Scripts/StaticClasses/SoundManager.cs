using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SoundManager {
    private static GameObject oneShotSound;
    private static AudioSource oneShotAudioSource;

    public static Dictionary<string, SoundTime> soundTimerDictionary;


    public class SoundTime {
        public float currentTime;
        public float maxTime;
    }

    public enum Sound {
        SlingShot,
        Health,
        BoxBreaking,
        EnemyHit,
        Victory,
        Loss,
        Explosion,
        Collision,
        ButtonClick,
    }


    public static void Playsound(Sound sound, int index = -1) {

        Playsound(sound, Camera.main.transform.position, index);
    }
    public static void Playsound(Sound sound, Vector3 position, int index = -1) {

        if (!CanPlaySound(sound)) {
            return;
        }
        GameObject soundGameobject = new GameObject("soundGameObject");
        soundGameobject.transform.position = position;
        AudioSource audioSource = soundGameobject.AddComponent<AudioSource>();
        Getsound(sound, audioSource, index);
        audioSource.Play();

        Object.Destroy(soundGameobject, audioSource.clip.length);
    }

    private static bool CanPlaySound(Sound sound) {
        string soundname = sound.ToString();
        if (soundTimerDictionary.ContainsKey(soundname)) {
            SoundTime Timer = soundTimerDictionary[soundname];


            if (Timer.currentTime + Timer.maxTime < Time.time) {
                soundTimerDictionary[soundname].currentTime = Time.time;
                return true;
            }

            return false;
        }

        return true;
    }
    private static void Getsound(Sound sound, AudioSource source, int index) {
        foreach (GameSoundAssets.SoundAudioClip soundaudioClip in GameSoundAssets.Instance.AudioClipsArray) {
            if (soundaudioClip.name == sound.ToString()) {
                if (index == -1) {
                    index = Random.Range(0, soundaudioClip.audioClips.Length);
                }
                source.clip = soundaudioClip.audioClips[index].clip;
                source.pitch = soundaudioClip.audioClips[index].pitch;
                source.time = soundaudioClip.audioClips[index].startTime;
                source.volume = soundaudioClip.audioClips[index].volume * soundaudioClip.masterVolume;

            }
        }

    }

    public static void AddbuttonSound(this Button buttonUI, Sound sound) {
        buttonUI.onClick.AddListener(() => SoundManager.Playsound(sound));
    }


}
