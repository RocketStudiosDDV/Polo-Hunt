using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RankingTable : MonoBehaviour
{
    private Transform position;
    //private Transform text;
    public InputRunnerModeMultiplayer [] penguins;
    public InputRunnerModeMultiplayer myPenguin;

    private void Awake()
    {
        position = transform.Find("Pos");
        //text = position.Find("Pos");

        
        //entryTemplate.gameObject.SetActive(false);
        //position.GetComponent<Text>().text = "warra";
        //float templateHeight = 30f;

        /*for (int i = 0; i < 10; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryRectTransform.gameObject.SetActive(true);
        }*/
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        /*
        penguins = FindObjectsOfType<InputRunnerModeMultiplayer>();
        myPenguin = FindObjectOfType<InputRunnerModeMultiplayer>();

        InputRunnerModeMultiplayer myPingu;
        foreach (InputRunnerModeMultiplayer pingu in FindObjectsOfType<InputRunnerModeMultiplayer>())
        {
            if (pingu.GetComponent<PhotonView>().IsMine)
            {
                myPingu = pingu;
            }
        }


        Debug.Log("num de pigus" + penguins.Length);
        string pos = "-1";
        double[] positions = new double [penguins.Length];

        if (penguins.Length > 1)
        {
            for (int i = 0; i < penguins.Length-1; i++)
            {
                for (int j = 0; j < penguins.Length-1; j++)
                {
                    if (penguins[j].gameObject.transform.position.z < penguins[j + 1].gameObject.transform.position.z)
                    {
                        InputRunnerModeMultiplayer penguin = penguins[j];
                        penguins[j] = penguins[j + 1];
                        penguins[j + 1] = penguin;
                    }
                }
            }
        }
        

        //gameObject.GetComponent<InputRunnerModeMultiplayer>().
        for (int i = 0; i < penguins.Length; i++)
        {          
            if (myPenguin.Equals( penguins[i]))
            {
                Debug.Log("SOY YO!!!!!");
                pos = (i + 1).ToString();
            }
        }
        */
        /*if (penguins.Length > 0)
        {
            Debug.Log("Mi pos" + myPenguin.transform.position);
            Debug.Log("Tu pos " + penguins[0].transform.position);
        }*/

        int myPosition = 1;
        if (penguins != null)
        {
            foreach (InputRunnerModeMultiplayer penguin in penguins)
            {
                if (penguin.gameObject.transform.position.z > myPenguin.gameObject.transform.position.z)
                {
                    myPosition++;
                }
            }
        }

        position.GetComponent<Text>().text = pos;
            
    }

}
