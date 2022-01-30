using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioFootstepPlayer : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip[] footstepClip;

    AudioClip RandomFootClip()
    {
        return footstepClip[Random.Range(0, footstepClip.Length)];
    }

    public void FootstepAudio()
    {
        footstepSource.PlayOneShot(RandomFootClip());
    }


}
