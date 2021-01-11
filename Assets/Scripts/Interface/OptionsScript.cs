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
        if (PlayerPrefs.GetInt("language") == 0)
        {
            int i = 0;
            foreach (Text t in Text)
            {
                
                if (t != null)
                {
                    t.text = EnglishTest[i];
                }
                i++;
            }
        }
        else
        {
            int i = 0;
            foreach (Text t in Text)
            {
                if (t != null)
                {
                    t.text = SpanishTest[i];
                }
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioMixer audioMixer;

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
        PlayerPrefs.SetInt("language", 0);
        for(int i = 0; i < 5; i++)
        {
            if (Text[i] != null)
            {
                Text[i].text = EnglishTest[i];
            }
        }
    }

    public void SetSpanish()
    {
        PlayerPrefs.SetInt("language", 1);
        for(int i = 0; i < 5; i++)
        {
            if (Text[i] != null)
            {
                Text[i].text = SpanishTest[i];
            }
        }
    }
}
