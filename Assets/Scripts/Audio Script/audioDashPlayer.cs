using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioDashPlayer : MonoBehaviour
{
    public AudioSource dashAudioSource;
 
    public void dashSound()
    {
        dashAudioSource.Play();
    }
}
