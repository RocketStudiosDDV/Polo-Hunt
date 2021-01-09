using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;

    private List<string> namePlayers;


    private void Awake()
    {
        Debug.Log("Holaaaaaaaaaaaa");

        namePlayers = new List<string>();
        namePlayers.Add("culos");
        namePlayers.Add("webos");
        namePlayers.Add("xixis");

        Debug.Log("size namePLayers " + namePlayers.Count);
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 30f;

        /*if(namePlayers.Count > 0)
        {*/
            for (int i = 0; i < namePlayers.Count; i++)
            {
                Transform entryTransform = Instantiate(entryTemplate, entryContainer);
                RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight*i);
                entryTransform.gameObject.SetActive(true);

                int rank = i + 1; //numero en el ranking
                string rankNumber; //nombre dle jugador

                switch (rank)
                {
                    case 1:
                        rankNumber = "1st";
                        break;
                    case 2:
                        rankNumber = "2nd";
                        break;
                    case 3:
                        rankNumber = "3rd";
                        break;
                    default:
                        rankNumber = rank + "th";
                        break;
                }

                entryTransform.Find("positionText").GetComponent<Text>().text = rankNumber;
                //entryTransform.Find("nameText").GetComponent<Text>().text = namePlayers[i];
                entryTransform.Find("nameText").GetComponent<Text>().text = namePlayers[i];
            }
        //}
    }

    //Recibe la lista de nicknames de los jugadores desdee online
    public void SetPlayerNames (List <string> names)
    {
        namePlayers = names;
    }

 
}
