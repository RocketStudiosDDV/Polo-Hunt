using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Almacena la información de partida.
/// Crea los pescados del mapa.
/// Decide quién es oso y quien es pinguino, y pide a MatchManager comenzar la partida cuando esten los jugadores listos.
/// </summary>
public class MatchInfo : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region VARIABLES
    // Información de partida
    public int numberOfBears = 1;   // nº de osos de la partida

    public int penguinsAlive; //pinguinos restantes
    public int bearsConnected; //osos conectados
    public int penguinsConnected; //pinguinos conectados

    private double _totalTime = 0; //tiempo que dura la partida

    public List<Player> playersList;    // lista de jugadores

    // Información de sincronización
    public List<bool> playersReady; // lista de jugadores listos
    private object penguinsCountLock;

    // Información de creación y sincronización de pescados
    public GameObject fishPrefab;
    private FishMultiplayer[] fishList;
    public List<Transform> fishPositions;

    // Referencias
    private MatchManager matchManager;

    #endregion
    #region UNITY CALLBACKS

    private void Awake()
    {
        // Inicializamos variables necesarias
        playersList = new List<Player>();
        playersReady = new List<bool>();
        matchManager = FindObjectOfType<MatchManager>();

        // Instanciamos los pescados
        fishList = new FishMultiplayer[fishPositions.Count];
        for (int i = 0; i < fishPositions.Count; i++)
        {
            Vector3 position = fishPositions[i].position;
            fishList[i] = (FishMultiplayer) Instantiate(fishPrefab, position, Quaternion.identity).GetComponent<FishMultiplayer>();
        }

        // Obtenemos los jugadores
        IEnumerator<Player> playerEnumerator = PhotonNetwork.CurrentRoom.Players.Values.GetEnumerator();
        while (playerEnumerator.MoveNext())
        {
            playersList.Add(playerEnumerator.Current);
            playersReady.Add(false);
        }

        // Decidimos el rol de cada uno y se lo anotamos en customProperties al jugador
        if (PhotonNetwork.IsMasterClient)
        {
            int bearsToAssign = numberOfBears;
            int penguinsToAssign = playersList.Count - numberOfBears;
            Debug.Log("Players= " + playersList.Count + " - Bears= " + bearsToAssign + " - Pingus=" + penguinsToAssign);
            foreach (Player player in playersList)
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                bool isPenguin = true;
                while (bearsToAssign > 0 || penguinsToAssign > 0)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        if (penguinsToAssign > 0)
                        {
                            isPenguin = true;
                            penguinsToAssign--;
                        }
                        else
                        {
                            isPenguin = false;
                            bearsToAssign--;
                        }
                    }
                    else
                    {
                        if (bearsToAssign > 0)
                        {
                            isPenguin = false;
                            bearsToAssign--;
                        }
                        else
                        {
                            isPenguin = true;
                            penguinsToAssign--;
                        }
                    }
                }
                hashtable.Add("isPenguin", isPenguin);
                player.SetCustomProperties(hashtable);
            }
        }        
        Debug.Log("Awake Finished");
    }

    // Update is called once per frame
    void Update()
    {

        endTime(Time.fixedTime);
        endGame();
        
    }

    #endregion
    #region PRIVATE METHODS
    
    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Actualiza el número de pinguinos vivos (thread-safe)
    /// </summary>
    [PunRPC]
    public void ActualiceNumPenguins()
    {
        lock(penguinsCountLock)
        {
            penguinsAlive--;
            if (penguinsAlive == 0)
                ShowResults();
        }
    }

    //modo espectador -> aunque haya game over que puedas seguir la camañar de la persona que te ha matado
    //le sigues y encima se ve un game over y a la dcha boton de espectar o salir
    //si le das a espectar se quita el game over y si le das a salir vas a main menu
    public void SpectatorMode(GameObject pinguino, GameObject oso)
    {
        //PAULA
        //intsnacia de la pinga muerta asignarle a camara la del oso y apaghar todos los controles
        //find el object of type y asignarla al oso
        //pinguino funcion muerte
        //pinguino al morir invoca a esta funcion

        /* Camera newCamera = pinguino.GetComponent<Camera>();
         newCamera = oso.GetComponent<Camera>();
         */
        Debug.Log("MODO ESPECTADOR");
    }


    //método que se llamará cuando el juego decida que se acabara la partida y se muestra a quien siga dntro de la partida
    //debe bloquear el input a todos los jugadores y enseñarles la pantalla de resultados, inferida de penguinsAlive y bearsConnected
    public void ShowResults()
    {
        //Paula / tomas
    }

    //inicia la partida
    public void StartGame()
    {
        _totalTime = _totalTime + 600; //le sumamos en segs el tiempo que dura la partida
                                       //inicializar posiciones de los pinguinos y cosas del mapa (pescados y así)

        //MANU
    }

    //Que ubique los requisitos de fin de partida
        //tiempo restante -> se acaba
        //Solo qudan pinguinos
    //El master avisa a todos de endMatch en caso de que el tiempo se haya acabado
    public void endGame()
    {

        //MANU 

        if (penguinsAlive == 0)
        {
            //se acaba Y SE LLAMARA A LA PUNTUACION - SHOW REUSLTS
        }

        if (penguinsConnected == 0)
        {
            //se acaba
        }

        if (bearsConnected == 0)
        {
            //se acaba
        }
    }
    //cuando se acaba el tiempo
    public void endTime(double deltaTime)
    {

        //MANU

        if (deltaTime > _totalTime)
        {
            //se acaba la partoida
        }
    }

    public void DestroyFish(int fishId)
    {
        if (fishList[fishId] != null)
        {
            Destroy(fishList[fishId]);
        }
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Player Property update");
        // Ponemos como listo al jugador que ha recibido el cambio de customProperties
        for(int i = 0; i < playersList.Count; i++)
        {
            Player player = playersList[i];
            if (targetPlayer.ActorNumber == player.ActorNumber)
            {
                playersReady[i] = true;
            }
        }

        bool allReady = true;
        foreach(bool ready in playersReady)
        {
            if (ready == false)
            {
                allReady = false;
            }
        }

        if (allReady)
        {
            Debug.Log("Starting match..");
            matchManager.InstantiatePlayers();
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
