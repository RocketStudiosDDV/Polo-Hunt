using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inGameAudioController : MonoBehaviour
{
    private AudioSource musicGame;

    void Start()
    {
        musicGame = GetComponent<AudioSource>();
        musicGame.Play();
    }
}
