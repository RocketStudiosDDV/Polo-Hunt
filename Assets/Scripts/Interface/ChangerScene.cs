using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangerScene : MonoBehaviour
{
    //public AudioSource sound;
    //public AudioClip soundMenu;
    public double timeEnd;
    public double timeStart;
    private bool click = false;
    private string name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeStart += Time.deltaTime;

        if ((timeStart > timeEnd) && (click == true))
        {
            SceneManager.LoadScene(name);
        }

    }

    public void ChangeScene(string nameScene)
    {
        timeEnd = timeStart + 1;
        name = nameScene;
        click = true;
    }

    public void OpenWebPage(string namePage)
    {
        Application.OpenURL(namePage);
        //sound.Play();
    }

    public void SoundButton()
    {
        //sound.clip = soundMenu;
        //sound.enabled = false;
        //sound.enabled = true;
    }
}
