using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangerScene : MonoBehaviour
{
    public AudioSource sound;
    public AudioClip soundMenu;
    public GameObject pelota;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void OpenWebPage(string namePage)
    {
        Application.OpenURL(namePage);
    }

    public void SoundButton()
    {
        sound.clip = soundMenu;
        sound.enabled = false;
        sound.enabled = true;
    }
}
