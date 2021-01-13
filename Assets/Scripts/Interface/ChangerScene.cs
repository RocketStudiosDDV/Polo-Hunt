using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangerScene : MonoBehaviour
{
    //public AudioSource sound;
    //public AudioClip soundMenu;
    public double timeEnd;
    public double timeStart;
    private bool click = false;
    private string name;

    public GameObject tutorialMain;
    public GameObject tutorialControls;
    public GameObject tutorialGamemodes;

    public GameObject tutorialPC;
    public GameObject tutorialGamepad;

    public GameObject tutorialPolo;
    public GameObject tutorialRace;

    public bool cambioControlesBool = false;
    public bool cambioGameModesBool = false;
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

    public void ActiveControls()
    {
        tutorialControls.SetActive(true);
        tutorialMain.SetActive(false);
    }

    public void ActiveGameModes()
    {
        tutorialGamemodes.SetActive(true);
        tutorialMain.SetActive(false);
    }

    public void CambioControles()
    {
        if (cambioControlesBool == false)
        {
            cambioControlesBool = true;
            tutorialPC.SetActive(false);
            tutorialGamepad.SetActive(true);
        }
        else
        {
            cambioControlesBool = false;
            tutorialPC.SetActive(true);
            tutorialGamepad.SetActive(false);
        }   
    }

    public void CambioGameModes()
    {
        if (cambioGameModesBool == false)
        {
            cambioGameModesBool = true;
            tutorialPolo.SetActive(false);
            tutorialRace.SetActive(true);
        }
        else
        {
            cambioGameModesBool = false;
            tutorialPolo.SetActive(true);
            tutorialRace.SetActive(false);
        }   
    }

    public void BackMain()
    {
        tutorialGamemodes.SetActive(false);
        tutorialControls.SetActive(false);
        tutorialMain.SetActive(true);
    }

}
