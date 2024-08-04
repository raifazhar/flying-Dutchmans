using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SoundManager
{
    private static GameObject oneShotSound;
    private static AudioSource oneShotAudioSource;

    public static Dictionary<string, SoundTime> soundTimerDictionary;
   

    public class SoundTime
    {
        public float currentTime;
        public float maxTime;
    }

    public enum Sound
    {
        SlingShot,
        health,
        BoxBreaking,
        EnemyHit,
        Victory,
        Loss,
        Explosion,
        Collision,
        ButtonClick,
    }


    public static void Playsound(Sound sound)
    {

        if (oneShotSound == null)
        {
            oneShotSound = new GameObject("One Shot Audio");
            oneShotAudioSource = oneShotSound.AddComponent<AudioSource>();
        }
        if (!CanPlaySound(sound))
        {
            return;
        }

        oneShotAudioSource.PlayOneShot(Getsound(sound,oneShotAudioSource));


    }
    public static void Playsound(Sound sound, Vector3 position)
    {
        if (!CanPlaySound(sound))
        {
            return;
        }
        GameObject soundGameobject = new GameObject("soundGameObject");
        soundGameobject.transform.position = position;
        AudioSource audioSource = soundGameobject.AddComponent<AudioSource>();
        audioSource.clip = Getsound(sound,audioSource);
        audioSource.Play();

        Object.Destroy(soundGameobject, audioSource.clip.length);
    }

    private static bool CanPlaySound(Sound sound)
    {
        string soundname = sound.ToString();
        if (soundTimerDictionary.ContainsKey(soundname))
        {
            SoundTime Timer = soundTimerDictionary[soundname];


            if (Timer.currentTime + Timer.maxTime< Time.time)
            {
                soundTimerDictionary[soundname].currentTime = Time.time ;
                return true;
            }

            return false;
        }

        return true;
    }
    private static AudioClip Getsound(Sound sound,AudioSource source)
    {
        foreach (GameSoundAssets.SoundAudioClip soundaudioClip in GameSoundAssets.Instance.AudioClipsArray)
        {
            if (soundaudioClip.name == sound.ToString())
            {
                Debug.Log(soundaudioClip.audioClips.Length);
                int index = Random.Range(0, soundaudioClip.audioClips.Length);
                Debug.Log(soundaudioClip.audioClips);
                source.pitch = soundaudioClip.audioClips[index].pitch;
                source.time = soundaudioClip.audioClips[index].startTime;
                return soundaudioClip.audioClips[index].clip ;
            }
        }
        Debug.Log("sound does not exist");
        return null;
    }


    public static void AddbuttonSound(this Button buttonUI, Sound sound)
    {
        buttonUI.onClick.AddListener(() => SoundManager.Playsound(sound));

    }
   

}
