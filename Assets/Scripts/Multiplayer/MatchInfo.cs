using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Almacena la información de partida. Contiene los eventos de inicio y fin de partida.
/// La información que es necesaria para llamar a dichos eventos está sincronizada entre jugadores.
/// La lógica que sólo debe ser ejecutada una vez la ejecuta únicamente el MasterClient
/// Decide quién es oso y quien es pinguino, y pide a MatchManager comenzar la partida cuando esten los jugadores listos.
/// Crea los pescados del mapa.
/// </summary>
public class MatchInfo : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region VARIABLES
    // Configuración de partida
    private int numberOfBears = 0;   // nº de osos de la partida (leído de customPreferences)
    private double matchLength = 60; //tiempo que dura la partida

    // Información de partida
    private GameMode gameMode;  // Modo de juego
    
    // - MODO HUNT
    public int penguinsAlive; //pinguinos restantes
    public int bearsConnected; //osos conectados
    public int penguinsConnected; //pinguinos conectados
    private double matchTime;   // momento actual de la partida
    public List<Player> playersList;    // Lista de jugadores
    private bool matchFinished;     // Se acabo la partida?
    private bool matchStarted;  // Se empezó la partida?
    // Modo espectador
    private bool spectating;   
    private Camera mainCamera;
    private GameObject clientPenguin;
    private GameObject killerBear;
    Vector3 spectatorOffset = new Vector3(0, -5, 10); // X hacia la izquierda, Y hacia abajo, Z hacia

    // - MODO RACE
    public List<string> clasification; // clasificación (modo race)
    public int penguinsNotFinished; // nº de pingüinos que aún no han terminado

    // Información de los ajustes
    private int language;

    // Información de sincronización
    public List<bool> playersReady; // lista de jugadores listos
    private object infoLock = new object(); // lock para actualizaciones que necesiten ser thread-safe
    private object clasificationLock = new object();    // lock para decidir la clasificación en modo race

    // Información de creación y sincronización de pescados
    public GameObject fishPrefab;   // Prefab a instanciar
    public List<Transform> fishPositions;   // Posiciones de los pescados (configurar en el inspector)
    private FishMultiplayer[] fishList; // Lista de pescados

    // Información de plataformas de hielo
    private List<IcePlatform> platformsList;    // Lista de plataformas de hielo

    // Referencias a HUD
    private GameObject hudTime;
    private Text hudTimeTxt;
    private GameObject hudPenguins;
    private Text hudPenguinsTxt;
    private GameObject hudResults;
    private Text hudResultsTxt;
    private Text hudResultsInfoTxt;
    private GameObject hudReturn;
    private Text hudReturnTxt;
    private Text hudReturnCountdownTxt;
    private GameObject hudAlert;
    private Text hudAlertTxt;
    private int timeToReturn;

    // Referencias
    private MatchManager matchManager;

    // TESTEO
    private LogWriter logWriter;
    #endregion

    #region UNITY CALLBACKS
    /// <summary>
    /// Inicializa variables (todos), instancia pescados (todos), asigna el rol de cada jugador (sólo el MasterClient)
    /// </summary>
    private void Awake()
    {
        // Inicializamos variables necesarias
        language = PlayerPrefs.GetInt("language", 0);
        playersList = new List<Player>();
        playersReady = new List<bool>();
        clasification = new List<string>();
        matchManager = FindObjectOfType<MatchManager>();
        fishPositions = matchManager.fishPositions;
        matchManager.matchInfo = this;
        matchFinished = false;
        matchStarted = false;
        spectating = false;
        penguinsNotFinished = PhotonNetwork.CurrentRoom.PlayerCount;
        logWriter = FindObjectOfType<LogWriter>();
        // Obtenemos las referencias al HUD
        foreach (Text text in Resources.FindObjectsOfTypeAll<Text>())
        {
            if (text.CompareTag("hudPenguinsAlive"))
            {
                hudPenguinsTxt = text;
                hudPenguins = hudPenguinsTxt.transform.parent.gameObject;
            }
            else if (text.CompareTag("hudTimeTxt"))
            {
                hudTimeTxt = text;
                hudTime = hudTimeTxt.transform.parent.gameObject;
            }
            else if (text.CompareTag("hudResultsTxt"))
            {
                hudResultsTxt = text;
                hudResults = hudResultsTxt.transform.parent.gameObject;
            }
            else if (text.CompareTag("hudResultsInfoTxt"))
            {
                hudResultsInfoTxt = text;
            }
            else if (text.CompareTag("hudReturnTxt"))
            {
                hudReturnTxt = text;
                hudReturn = hudReturnTxt.transform.parent.gameObject;
                hudReturnCountdownTxt = hudReturnTxt.GetComponentsInChildren<Text>()[1];
            }
            else if(text.CompareTag("hudAlertTxt"))
            {
                hudAlertTxt = text;
                hudAlert = hudAlertTxt.transform.parent.gameObject;
            }
        }
        // Pintamos esperando por jugadores...
        if (hudResults != null)
        {
            hudResults.SetActive(true);
            if (language == 0)
            {
                hudResultsInfoTxt.text = "Waiting for players...";
            }
            else
            {
                hudResultsInfoTxt.text = "Esperando por los jugadores...";
            }
        }            


        // Leemos ajustes de partida de las CustomProperties de la Room
        object customPropertyBears;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("numberOfBears", out customPropertyBears))             // nº de osos
        {
            numberOfBears = (int)customPropertyBears;
        }
        while (numberOfBears >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            numberOfBears--;
        }
        if (numberOfBears <= 0)
        {
            numberOfBears = 1;
        }
        object customPropertyGameMode;
        gameMode = matchManager.gameMode;


        // Instanciamos los pescados
        Quaternion angleFish;
        Vector3 sizeFish;

        fishList = new FishMultiplayer[fishPositions.Count];
        if (gameMode == GameMode.Hunt)
        {
            angleFish = Quaternion.Euler(-90f, 0f, 0f);
            sizeFish = new Vector3(1, 1, 1);
        }
        else
        {
            angleFish = Quaternion.Euler(-90f, 0f, 0f);
            sizeFish = new Vector3(3, 3, 3);
        }

        for (int i = 0; i < fishPositions.Count; i++)
        {
            Vector3 position = fishPositions[i].position;
            fishList[i] = Instantiate(fishPrefab, position, angleFish).GetComponent<FishMultiplayer>();
            fishList[i].transform.localScale = sizeFish;
            fishList[i].id = i;
        }

        // Obtenemos las plataformas de hielo
        platformsList = new List<IcePlatform>();
        foreach(IcePlatform platform in FindObjectsOfType<IcePlatform>())
        {
            platformsList.Add(platform);
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
            bearsConnected = numberOfBears;
            int penguinsToAssign = PhotonNetwork.CurrentRoom.PlayerCount - numberOfBears;
            penguinsConnected = penguinsToAssign;
            penguinsAlive = penguinsConnected;
            int playerId = 0;
            foreach (Player player in playersList)
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                bool isPenguin = true;
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
                hashtable.Add("isPenguin", isPenguin);
                hashtable.Add("playerId", playerId);
                hashtable.Add("alive", true);
                player.SetCustomProperties(hashtable);
                playerId++;
            }
            object[] objectArray = new object[2];
            objectArray[0] = penguinsConnected;
            objectArray[1] = bearsConnected;
            GetComponent<PhotonView>().RPC("SetNumberOfPlayers", RpcTarget.All, objectArray as object);
        }
    }

    /// <summary>
    /// Mientras esté la partida en marcha, comprueba los eventos
    /// </summary>
    void Update()
    {
        if (!matchFinished && matchStarted && gameMode == GameMode.Hunt)
        {
            endTime(Time.deltaTime);
        }
        if (spectating)
        {
            mainCamera.transform.position = killerBear.transform.position - new Vector3(0, -3.5f, 0) - killerBear.transform.forward * 15;
            mainCamera.transform.LookAt(killerBear.transform.position);
            mainCamera.transform.Rotate(-20f, 0f, 0f);
        }
    }
    #endregion

    #region RPCs
    /// <summary>
    /// Destruye el pescado con el identificador fishId
    /// </summary>
    /// <param name="fishId"></param>
    [PunRPC]
    public void DestroyFish(object parameter)
    {
        int fishId = (int)parameter;
        if (fishList[fishId] != null)
        {            
            Destroy(fishList[fishId].gameObject);
        }
    }

    /// <summary>
    /// Destruye la plataforma con el identificador platformId
    /// </summary>
    /// <param name="parameter"></param>
    [PunRPC]
    public void DestroyPlatform(object parameter)
    {
        Debug.Log("destruye plat" + (int)parameter);
        int platformId = (int)parameter;
        IcePlatform platformToDestroy = null;
        foreach(IcePlatform platform in platformsList)
        {
            if (platform.platformId == platformId)
            {
                platformToDestroy = platform;
            }
        }
        if (platformToDestroy != null)
        {
            platformsList.Remove(platformToDestroy);
            Destroy(platformToDestroy.gameObject);
        }
    }

    /// <summary>
    /// Avisa al Master de que ha llegado a la meta
    /// </summary>
    [PunRPC]
    public void GoalReached(object parameters)
    {
        string name = (string)parameters;
        clasification.Add(name);
        GetComponent<PhotonView>().RPC("UpdateClasification", RpcTarget.All, (object)clasification.ToArray());
    }

    /// <summary>
    /// Avisa a todos los jugadores de que actualicen su pantalla de resultados
    /// Si han llegado todos, pone el contador de volver a la sala de selección de partida
    /// </summary>
    /// <param name="parameters"></param>
    [PunRPC]
    public void UpdateClasification(object parameters)
    {
        clasification.Clear();
        foreach (string str in (string[])parameters)
        {
            clasification.Add(str);
        }
        HighscoreTable table = FindObjectOfType<HighscoreTable>();
        if (table != null)
        {
            table.SetPlayerNames(clasification);
            table.ActualizeClasification();
        }
        penguinsNotFinished--;
        if (penguinsNotFinished <= 0)
        {
            ShowReturnHUD(20);
        }
    }

    /// <summary>
    /// Actualiza el número de pinguinos vivos para todos los jugadores (thread-safe)
    /// Si no quedan vivos, termina la partida
    /// </summary>
    [PunRPC]
    public void ActualizeNumPenguins()
    {
        lock (infoLock)
        {
            penguinsAlive--;
            if (hudPenguinsTxt != null)
                hudPenguinsTxt.text = "" + penguinsAlive;
            Debug.Log("penguins alive = " + penguinsAlive);
            if (penguinsAlive == 0)
                ShowResults();
        }
    }

    /// <summary>
    /// Actualiza el número de pingüinos vivos y conectados para todos los jugadores (thread-safe)
    /// Si no quedan vivos o conectados, termina la partida
    /// </summary>
    [PunRPC]
    public void ActualizeNumPenguinsConnected()
    {
        lock (infoLock)
        {
            penguinsConnected--;
            if (penguinsConnected == 0 || penguinsAlive == 0)
                ShowResults();
        }
    }

    /// <summary>
    /// Actualiza el número de osos conectados (thread-safe)
    /// Si no quedan conectados, termina la partida
    /// </summary>
    [PunRPC]
    public void ActualizeNumBearsConnected()
    {
        lock (infoLock)
        {
            bearsConnected--;
            if (bearsConnected == 0)
                ShowResults();
        }
    }

    /// <summary>
    /// Sincroniza la información de nº de jugadores conectados y roles e inicia la partida
    /// </summary>
    /// <param name="objectArray"></param>
    [PunRPC]
    public void SetNumberOfPlayers(object[] objectArray)
    {
        int numberOfPenguins = (int)objectArray[0];
        int numberOfBears = (int)objectArray[1];
        lock (infoLock)
        {
            penguinsAlive = numberOfPenguins;
            penguinsConnected = numberOfPenguins;
            bearsConnected = numberOfBears;
        }
    }

    /// <summary>
    /// Instancia cada uno su jugador
    /// </summary>
    [PunRPC]
    public void InstantiatePlayers()
    {
        matchStarted = true;
        ShowResultsHUD(false);
        ShowInGameHUD(true);
        if (hudPenguinsTxt != null)
            hudPenguinsTxt.text = "" + penguinsAlive;
        matchManager.InstantiatePlayers();
    }
    #endregion

    #region PUBLIC METHODS    
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
        mainCamera = FindObjectOfType<Camera>();
        killerBear = oso;
        spectating = true;
    }

    // Lo mismo que el anterior pero para los que entran en mitad de partida (no tienen pinguino ni oso que les haya matado)
    public void SpectatorMode()
    {
        Debug.Log("MODO ESPECTADOR");
    }


    /// <summary>
    /// Muestra/oculta el HUD in game
    /// </summary>
    /// <param name="show"></param>
    public void ShowInGameHUD(bool show)
    {
        if (hudTime == null)
            return;
        hudTime.SetActive(show);
        hudPenguins.SetActive(show);
    }

    /// <summary>
    /// Muestra/oculta el HUD de los resultados
    /// </summary>
    /// <param name="show"></param>
    public void ShowResultsHUD(bool show)
    {
        if (hudResults == null)
            return;
        hudResults.SetActive(show);
    }

    /// <summary>
    /// Muestra el mensaje de alerta message durante el tiempo time
    /// </summary>
    /// <param name="time"></param>
    /// <param name="message"></param>
    public void ShowAlertHUD(int time, string message)
    {
        if (hudAlert == null)
            return;
        hudAlert.SetActive(true);
        hudAlertTxt.text = message;
        Invoke(nameof(HideAlertHUD), time);
    }

    /// <summary>
    /// Oculta el mensaje de alerta
    /// </summary>
    public void HideAlertHUD()
    {
        if (hudAlert == null)
            return;
        hudAlert.SetActive(false);
        hudAlertTxt.text = "";
    }

    /// <summary>
    /// Muestra el HUD de volver al menú y devuelve al menú en timeToReturn segundos
    /// </summary>
    /// <param name="countdown"></param>
    public void ShowReturnHUD(int countdown)
    {
        if (hudReturn == null)
            return;
        hudReturn.SetActive(true);
        if (language == 0)
        {
            hudReturnTxt.text = "Returning to room...";
        } else
        {
            hudReturnTxt.text = "Volviendo a la sala en...";
        }
        timeToReturn = countdown;
        hudReturnCountdownTxt.text = timeToReturn.ToString();
        Invoke(nameof(ActualizeCountdown), 1f);
    }

    private void ActualizeCountdown()
    {
        timeToReturn--;
        if (timeToReturn <= 0)
        {
            FindObjectOfType<ConectionManagerInGame>().ReturnToGameSelectionMenu();
        } else
        {
            Invoke(nameof(ActualizeCountdown), 1f);
            hudReturnCountdownTxt.text = timeToReturn.ToString();
        }
    }

    //método que se llamará cuando el juego decida que se acabara la partida y se muestra a quien siga dntro de la partida
    //debe bloquear el input a todos los jugadores y enseñarles la pantalla de resultados, inferida de penguinsAlive y bearsConnected
    public void ShowResults()
    {
        if (matchFinished == false)
        {
            ShowInGameHUD(false);
            ShowResultsHUD(true);
            ShowReturnHUD(10);
            matchFinished = true;
            if (logWriter != null)
                logWriter.Write("SE ACABO LA PARTIDA");
            if (bearsConnected == 0 && gameMode == GameMode.Hunt)
            {
                if (logWriter != null)
                    logWriter.Write("se fueron todos los osos");
                // TODO - decir victoria a los pingüinos vivos y mostrar la pantalla de fin de partida
                object isAlive = false;
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("alive", out isAlive);
                if ((bool)isAlive)
                {
                    if (language == 0)
                    {
                        hudResultsTxt.text = "VICTORY";
                        hudResultsInfoTxt.text = "All bears disconnected";
                    }
                    else
                    {
                        hudResultsTxt.text = "VICTORIA";
                        hudResultsInfoTxt.text = "Se han desconectado todos los osos";
                    }
                }
                // TODO - decir volviendo en X segundos a la sala de selección de partida y llamar a ConectionManagerInGame.ReturnToGameSelectionMenu()
            }
            else if (penguinsConnected == 0)
            {
                if (logWriter != null)
                    logWriter.Write("se fueron todos los pinguinos");
                // TODO - decir victoria a los osos y mostrar la pantalla de fin de partida
                if (language == 0)
                {
                    hudResultsTxt.text = "VICTORY";
                    hudResultsInfoTxt.text = "All penguins disconnected";
                }
                else
                {
                    hudResultsTxt.text = "VICTORIA";
                    hudResultsInfoTxt.text = "Se han desconectado todos los pingüinos";
                }
                // TODO - decir volviendo en X segundos a la sala de selección de partida y llamar a ConectionManagerInGame.ReturnToGameSelectionMenu()
            }
            else if (penguinsAlive == 0)
            {
                if (logWriter != null)
                    logWriter.Write("todos los pinguinos han sido cazados");
                // TODO - decir victoria a los osos y mostrar pantalla de fin de partida
                // TODO - decir volviendo en X segundos a la sala de selección de partida y llamar a ConectionManagerInGame.ReturnToGameSelectionMenu()
                object isPenguin = false;
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out isPenguin);
                if (language == 0)
                {
                    hudResultsInfoTxt.text = "All penguins have been hunted";
                } else
                {
                    hudResultsInfoTxt.text = "Todos los pingüinos han sido cazados";
                }
                if ((bool)isPenguin)
                {
                    if (language == 0)
                    {
                        hudResultsTxt.text = "DEFEAT";
                    } else
                    {
                        hudResultsTxt.text = "DERROTA";
                    }
                } else
                {
                    if (language == 0)
                    {
                        hudResultsTxt.text = "VICTORY";
                    }
                    else
                    {
                        hudResultsTxt.text = "VICTORIA";
                    }
                }
            }
            else
            {
                if (logWriter != null)
                    logWriter.Write("se acabo el tiempo");
                // TODO - decir victoria a los pingüinos vivos y mostrar pantalla de fin de partida
                // TODO - decir volviendo en X segundos a la sala de selección de partida y llamar a ConectionManagerInGame.ReturnToGameSelectionMenu()
                object isAlive = false;
                object isPenguin = false;
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("alive", out isAlive);
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out isPenguin);                
                if ((bool)isPenguin)
                {
                    if ((bool)isAlive)
                    {
                        if (language == 0)
                        {
                            hudResultsTxt.text = "VICTORY";
                            hudResultsInfoTxt.text = "You survived the hunt";
                        } else
                        {
                            hudResultsTxt.text = "VICTORIA";
                            hudResultsInfoTxt.text = "Has sobrevivido a la cacería";
                        }
                    } else
                    {
                        if (language == 0)
                        {
                            hudResultsTxt.text = "DEFEAT";
                            hudResultsInfoTxt.text = "You've been hunted";
                        } else
                        {
                            hudResultsTxt.text = "DERROTA";
                            hudResultsInfoTxt.text = "Has sido cazado";
                        }
                    }
                } else
                {
                    if (language == 0)
                    {
                        hudResultsTxt.text = "DEFEAT";
                        hudResultsInfoTxt.text = "There are penguins left";
                    } else
                    {
                        hudResultsTxt.text = "DERROTA";
                        hudResultsInfoTxt.text = "No han sido cazados todos los pingüinos";
                    }
                }
            }
        }       
    }

    /// <summary>
    /// Comprueba si se ha terminado el tiempo de partida
    /// Si se da el caso, termina la partida
    /// </summary>
    /// <param name="deltaTime"></param>
    public void endTime(double deltaTime)
    {
        if (matchStarted)
        {
            matchTime += deltaTime;
        }
        if (gameMode == GameMode.Hunt)
        {
            hudTimeTxt.text = ((int)(matchLength - matchTime) / 60).ToString() + ": " + ((int)(matchLength - matchTime) % 60).ToString();
        }
        if (matchTime > matchLength)
        {
            ShowResults();
        }
    }
    #endregion

    #region PRIVATE METHODS
    /// <summary>
    /// Wrapper para método RPC para poder invocarlo con delay
    /// </summary>
    private void InstantiatePlayersWrapper()
    {
        GetComponent<PhotonView>().RPC("InstantiatePlayers", RpcTarget.All);
    }
    #endregion

    #region PUN CALLBACKS
    /// <summary>
    /// Si un jugador entra en mitad de partida, ponerlo en modo espectador
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    /// <summary>
    /// Si un jugador sale de la partida, actualiza el nº de jugadores conectados
    /// Si estaba vivo, actualiza el nº de pingüinos vivo
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (language == 0)
        {
            ShowAlertHUD(5, otherPlayer.NickName + " left the room.");
        } else
        {
            ShowAlertHUD(5, otherPlayer.NickName + " se fue de la sala.");
        }
        if (PhotonNetwork.IsMasterClient)
        {
            object wasPenguin;
            otherPlayer.CustomProperties.TryGetValue("isPenguin", out wasPenguin);
            if ((bool)wasPenguin)
            {
                GetComponent<PhotonView>().RPC("ActualizeNumPenguinsConnected", RpcTarget.All);
                object wasAlive = false;
                otherPlayer.CustomProperties.TryGetValue("alive", out wasAlive);
                if ((bool)wasAlive)
                    GetComponent<PhotonView>().RPC("ActualizeNumPenguins", RpcTarget.All);
            } else if (gameMode == GameMode.Hunt)
            {
                GetComponent<PhotonView>().RPC("ActualizeNumBearsConnected", RpcTarget.All);
            }
        }
        if (gameMode == GameMode.Race)  // Si es modo carrera y no había llegado a la meta lo descontamos de la lista
        {
            object goalReachedProp = false;
            otherPlayer.CustomProperties.TryGetValue("goalReached", out goalReachedProp);
            if (!(bool)goalReachedProp)
            {
                penguinsNotFinished--;
                if (penguinsNotFinished <= 0)
                {
                    ShowReturnHUD(10);
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// Recibe cada vez que se le asigna el rol a cada jugador y lo marca como listo.
    /// Una vez estén todos listos, comunica a todos que empiecen la partida
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object property = true;
        if (changedProps.TryGetValue("alive", out property))
        {
            if ((bool)property)
            {
                for (int i = 0; i < playersList.Count; i++)
                {
                    Player player = playersList[i];
                    if (targetPlayer.ActorNumber == player.ActorNumber)
                    {
                        playersReady[i] = true;
                    }
                }

                bool allReady = true;
                foreach (bool ready in playersReady)
                {
                    if (ready == false)
                    {
                        allReady = false;
                    }
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    if (allReady)
                    {
                        Invoke("InstantiatePlayersWrapper", 3f);
                    }
                }
                Invoke("AssignRankingTableValues", 6f);
            }
        }                
    }

    public void AssignRankingTableValues()
    {
        RankingTable rankingTable = FindObjectOfType<RankingTable>();
        if (rankingTable != null)
            rankingTable.penguins = FindObjectsOfType<InputRunnerModeMultiplayer>();
    }
    #endregion

}
