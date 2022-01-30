using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioJumpPlayer : MonoBehaviour
{
    public AudioSource jumpfrontSource;
    public AudioSource jumpbackSource;
   
    public void JumpFront()
    {
        jumpfrontSource.Play();
    }

    public void JumpBack()
    {
        jumpbackSource.Play();
    }




}


    
