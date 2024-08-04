using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonExtension : MonoBehaviour
{
    public SoundManager.Sound soundToPlay;
    public int index = -1;
    public void AddSound()
    {
        SoundManager.Playsound(soundToPlay,index);
    }
}
