﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardMobile : MonoBehaviour
{

    public InputField inp;
    public ConectionManager conectionManager;

    private TouchScreenKeyboard keyboard;
    private bool inputAssigned;

    // Start is called before the first frame update

    public void openKeyboard(){
        if (Application.isMobilePlatform)
            keyboard = TouchScreenKeyboard.Open("",TouchScreenKeyboardType.Default);
    }

    void Start()
    {
        inputAssigned = false;
        if (inp == null){
            inp = GameObject.Find("SignUpNicknameInp").GetComponent<InputField>();
            if (inp == null)Debug.Log("OKE");
            else Debug.Log("NOT OKE");
        }
        if (Application.isMobilePlatform){
            inp.text = "player";
            conectionManager.Connect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!TouchScreenKeyboard.visible && inputAssigned == false){
            if (keyboard != null)
            {
                if (keyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    inp.text = keyboard.text;
                    inputAssigned = true;
                }
            }
        }
    }
}
