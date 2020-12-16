using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioMixer audioMixer;

    public int English = 0;
    public string[] EnglishTest;
    public string[] SpanishTest;

    public Text[] Text;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetBright(float bright)
    {
        Debug.Log(bright);
    }

    public void SetEnglish()
    {
        English = 0;
        for(int i = 0; i < 8; i++)
        {
            if (Text[i] != null)
            {
                Text[i].text = EnglishTest[i];
            }
        }
    }

    public void SetSpanish()
    {
        English = 1;
        for(int i = 0; i < 8; i++)
        {
            if (Text[i] != null)
            {
                Text[i].text = SpanishTest[i];
            }
        }
    }
}
