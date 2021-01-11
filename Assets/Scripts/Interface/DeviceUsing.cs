using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUsing : MonoBehaviour
{
    private Canvas canvasHUD;
    private GameObject mobileHUD;

    void Start()
    {
        canvasHUD = GetComponent<Canvas>();
        mobileHUD = canvasHUD.transform.Find("InputMobile").gameObject as GameObject;

        /*if (SystemInfo.deviceType == DeviceType.Handheld) //si es movil
        {
            Debug.Log("SOY UN MOVIL");
            mobileHUD.SetActive(true);
        }
        else //es ordenador
        {
            Debug.Log("SOY UN ORDENAODR");
            mobileHUD.SetActive(false);
        }*/


        /*if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("SOY UN MOVIL");
            mobileHUD.SetActive(true);
        }

        if ((Application.platform == RuntimePlatform.WindowsPlayer)||(Application.isMobilePlatform)
        {
            Debug.Log("SOY UN ORDENAODR");
            mobileHUD.SetActive(false);
        }*/

        if ((Application.isMobilePlatform))
        {
            Debug.Log("SOY UN MOVIL");
            mobileHUD.SetActive(true);
        }
        else
        {
            Debug.Log("SOY UN ORDENAODR");
            mobileHUD.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
