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
            Debug.Log(Timer);

            if (Timer.currentTime + Timer.maxTime< Time.time)
            {
                Debug.Log("Playsound again");
                soundTimerDictionary[soundname].currentTime = Time.time ;
                return true;
            }

            return false;
        }

        return true;
    }
    private static AudioClip Getsound(Sound sound,AudioSource source)
    {
        foreach (GameSoundAssets.SoundAudioClip soundaudioClip in GameSoundAssets.Instance.AudioClips)
        {
            if (soundaudioClip.name == sound.ToString())
            {
                int index = Random.Range(0, soundaudioClip.sounds.Length);
                return soundaudioClip.sounds[index];
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
