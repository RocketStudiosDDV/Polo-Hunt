using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputRunnerModeMultiplayer : MonoBehaviour
{
    #region VARIABLES

    //CONTROL DEL PLAYER
    //[SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 25;
    //[SerializeField] Canvas finalRanking;
    //[SerializeField] float force = 50;


    private PlayerControls _controls;
    private Rigidbody _playerRB; //Rigid body del pingu

    private Vector2 _horizontaldirection;
    private Vector3 playerInput; //guarda la info del input
    private Vector3 playerDirection; //Hacia donde se mueve el jugador

    //CONTROL DE LA CÁMARA
    public Camera mainCamera; //Guarda cuál es nuestra cámara
    private Vector3 camFordward; //vector camara hacia delante
    private Vector3 camRight; //camara hacia la dcha, direccion a la que mira la camara

    public Vector3 offset;
    public Vector2 movementCamera;
    public Transform target;
    public Transform pivot;
    private float moveX;
    private float moveY;
    private float m_LookSense = 0.5f;

    private bool fishEaten = false;
    private double _timeFish;

    private bool isHitted = false;

    //CONTROL DE ANIMACIONES
    Animator penguin_animator;

    private bool walking_animation = true;
    private bool sliding_animation = false;

    //rotacion inicio 
    private bool toStart = false;
    private double _timeStart;

    //empujon
    //private double _timeAttacking;
    //private bool isAttacking = false;

    private bool toStop = false;
    private double _timeTillStop;

    public Vector3 penguinPos; //guarda la posicion por si el pingu se cae para respawnearlo ahi
    public bool inFloor = true; //comprueba si esta en el suelo o se ha caído

    private bool snowmanCollided = false;
    private double _timeSnowman;

    private bool fenceTriggered = false;
    private double _timeFence;

    private bool obstacle = false;
    private double _timeSlow = 0;

    private bool onObstacleMini = false;
    private double _timeObstacle;

    private bool onRamp = false;
    //private double _timeStopped = 0;

    private bool finished = false;
    private double timeRanking;
    public Canvas canvasHUD;
    //public Transform canvasNameHUD;
    public GameObject tableRanking;
    public GameObject finalText;
    public GameObject posInGame;

    public GameObject PauseButton;
    public bool PauseButtonActivo = false;

    private MatchInfo matchInfo;

    // ONLINE
    public GameObject pivotPrefab;
    public List<string> clasification;
    private bool goalReachedSent;   // Ha enviado el mensaje de meta alcanzada? (modo RACE)

    //EFECTOS DE AUDIO
    private AudioSource penguinPlayer;
    public AudioClip eatFishEffect;
    public AudioClip iceDashEffect;
    public AudioClip collideObject;
    public AudioClip arriveGoal;
    public AudioClip dash;

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls();
        clasification = new List<string>();

        // Inicialización de variables
        goalReachedSent = false;

        if (GetComponent<PhotonView>().IsMine || !PhotonNetwork.IsConnected)  // Si es nuestro pingüino, seguirlo con la cámara
        {
            mainCamera = FindObjectOfType<Camera>();
            pivot = Instantiate(pivotPrefab).transform;
            target = pivot.GetChild(0).transform;

            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("goalReached", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

        }

        foreach(Button b in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (b.gameObject.CompareTag("PAUSE"))
                PauseButton = b.gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        //penguinPlayer = GetComponent<AudioSource>();

        foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (canvas.CompareTag("RunnerHUD"))
            {
                canvasHUD = canvas;
            }
        }

        foreach (AudioSource audiosource in Resources.FindObjectsOfTypeAll<AudioSource>())
        {
            if (audiosource.CompareTag("AudioEffects"))
            {
                penguinPlayer = audiosource;
            }
        }

        tableRanking = canvasHUD.transform.Find("HighscoreTable").gameObject as GameObject;
        finalText = canvasHUD.transform.Find("FinalText").gameObject as GameObject;
        posInGame = canvasHUD.transform.Find("Position").gameObject as GameObject;

        _playerRB = GetComponent<Rigidbody>();
        matchInfo = FindObjectOfType<MatchInfo>(); //si muere llamar a matchInfo.SpectatorMode
        penguin_animator = GetComponent<Animator>();


        //correr y moverse deben estar unabled antes del inicio
        _controls.Player.Run.Disable();
        _controls.Player.Movement.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetComponent<Rigidbody>().velocity);
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        if (walking_animation == true)
        {
            //Debug.Log("A andar");
            _playerRB.AddForce(Vector3.forward * 2.25f * Time.deltaTime * 500, ForceMode.Acceleration);
        }

        penguin_animator.SetBool("walking", walking_animation);
        penguin_animator.SetBool("sliding", sliding_animation);

    }

    private void FixedUpdate()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        playerInput = new Vector3(_horizontaldirection.x, 0, 1);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      

        _playerRB.transform.LookAt(_playerRB.transform.position + playerInput);

        CameraDirection();
        ThirdCamera();


        target.transform.LookAt(pivot);

        FishRun(Time.fixedTime); //Velocidad despues de comer el pez

        //movimiento previo
        if (toStart == true)
        {
            if (Time.fixedTime > _timeStart)
            {
                _playerRB.GetComponentInChildren<CapsuleCollider>().transform.rotation = Quaternion.Euler(80f, _playerRB.transform.rotation.y, 0f); //Controla que el collider se mueva con el `pingo
                _playerRB.GetComponentInChildren<CapsuleCollider>().transform.position = new Vector3(_playerRB.transform.position.x, _playerRB.transform.position.y + 0.25f, _playerRB.transform.position.z - 0.3f);
                _controls.Player.Run.Enable();
                _controls.Player.Movement.Enable();
                toStart = false;
            }
        }

        //movimiento durante        
        if ((walking_animation == false) && (isHitted == false))
        {
            if (toStop == false)
            {
                _playerRB.velocity = new Vector3(playerInput.x * speed, _playerRB.velocity.y, playerInput.z * speed);
                _playerRB.AddForce(Vector3.up * -100, ForceMode.Force); //hace fuerza hacia abajo de forma que no vuele
                _playerRB.transform.rotation = Quaternion.Euler(20f, _playerRB.velocity.x, 0f);
            }
            else
            {
                if (Time.fixedTime < _timeTillStop)
                {
                    _playerRB.AddForce(Vector3.forward * 3, ForceMode.Acceleration);
                    _playerRB.velocity = new Vector3(playerInput.x * speed, _playerRB.velocity.y, playerInput.z * speed);
                }
                else
                {
                    Debug.Log("slide false");
                    sliding_animation = false;
                    finished = true;

                    posInGame.SetActive(false);
                    finalText.SetActive(true);
                    ShowRanking();

                    if (Time.fixedTime > timeRanking)
                    {
                        //PASAR A LA SALA AGAIN
                    }
                }
            }
        }

        //Choque con snowman
        if (snowmanCollided == true)
        {
            if ((Time.fixedTime > _timeSnowman - 3) && (Time.fixedTime < _timeSnowman - 2))
            {
                speed = 10;
            }

            if ((Time.fixedTime > _timeSnowman - 2) && (Time.fixedTime < _timeSnowman - 1))
            {
                speed = 15;
            }

            if ((Time.fixedTime > _timeSnowman - 1) && (Time.fixedTime < _timeSnowman))
            {
                speed = 25;
            }
        }

        //choque con valla
        if (fenceTriggered == true)
        {
            speed = 15;

            if (Time.fixedTime > _timeFence)
            {
                speed = 25;
                fenceTriggered = false;
            }
        }

        //choque con cosas del mapa
        if (obstacle == true)
        {
            if (Time.fixedTime > _timeSlow)
            {
                speed = 25;
                obstacle = false;
            }
        }

        if (onObstacleMini == true)
        {
            if (Time.fixedTime > _timeObstacle)
            {
                speed = 25;
                onObstacleMini = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (collision.gameObject.tag == "RightSky") //Si esta en el mapa
        {
            inFloor = false;
            penguinPos = _playerRB.position;
            _playerRB.MovePosition(new Vector3(penguinPos.x - 4, penguinPos.y + 10, penguinPos.z)); //la coloca en medio, si la cuesta es recta no hace falta dcha izq, sino si
            Debug.Log("ME CAIGO");
        }

        if (collision.gameObject.tag == "LeftSky") //Si esta en el mapa
        {
            inFloor = false;
            penguinPos = _playerRB.position;
            _playerRB.MovePosition(new Vector3(penguinPos.x + 4, penguinPos.y + 10, penguinPos.z)); //la coloca en medio, si la cuesta es recta no hace falta dcha izq, sino si
            Debug.Log("ME CAIGO");
        }

        if (collision.gameObject.tag == "Goal") //LLEGA A LA META
        {
            penguinPlayer.clip = arriveGoal;
            penguinPlayer.Play();

            int numRand = Random.Range(2, 6);
            Debug.Log(numRand);
            _timeTillStop = Time.fixedTime + numRand;
            timeRanking = Time.fixedTime + numRand + 2;
            toStop = true;
            if (GetComponent<PhotonView>().IsMine)  // Si es el dueño del pingüino
            {
                if (goalReachedSent == false)   // Y no ha avisado de que ha llegado a la meta
                {
                    goalReachedSent = true;
                    object parameters = PhotonNetwork.LocalPlayer.NickName;
                    matchInfo.GetComponent<PhotonView>().RPC("GoalReached", RpcTarget.MasterClient, parameters);    // Avisa al master de que ha llegado
                }
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                hashtable.Add("goalReached", true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
            }
            Debug.Log("Oleee");
        }

        if (collision.gameObject.tag == "LeftPenguin") //Si choca con el lado izq de un pingu
        {
            isHitted = true;
            WasHitted(false);
        }

        if (collision.gameObject.tag == "RightPenguin") //Si choca con el lado dch de un pingu
        {
            isHitted = true;
            WasHitted(true);
        }

        if (collision.gameObject.tag == "Start") //Si choca con el lado dch de un pingu
        {
            //finalRanking.gameObject.SetActive(true);
            //finalRanking.enabled = true;
            //ShowRanking();
            walking_animation = false;
            speed = 25;
            sliding_animation = true;
            _timeStart = Time.fixedTime + 1.5;
            toStart = true;
        }

        if (collision.gameObject.tag == "Fence") //Si choca con el lado dch de un pingu
        {
            penguinPlayer.clip = collideObject;
            penguinPlayer.Play();

            fenceTriggered = true;
            _timeFence = Time.fixedTime + 3;
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (collision.gameObject.tag == "Fish") //Si choca con un pescao
        {
            fishEaten = true;
            speed = 45;
            _timeFish = Time.fixedTime + 3;
            //Debug.Log("COLAS");
            penguinPlayer.clip = eatFishEffect;
            penguinPlayer.Play();

        }

        if (collision.gameObject.tag == "RestZone") //Si llega al descanso
        {
            Debug.Log("DESCANSO");

        }

        if (collision.gameObject.tag == "Floor") //Si esta en el mapa
        {

            Debug.Log("SUELO");

            if (onRamp == true)
            {
                speed = 25;
            }
            //_playerRB.AddForce(Vector3.left * 10, ForceMode.Acceleration);
            //_playerRB.velocity = new Vector3(playerInput.x * speed, _playerRB.velocity.y, playerInput.z * speed);
            inFloor = true;
        }


        if (collision.gameObject.tag == "Snowman") //Si choca con el lado dch de un pingu
        {
            penguinPlayer.clip = collideObject;
            penguinPlayer.Play();

            speed = 8;
            snowmanCollided = true;
            _timeSnowman = Time.fixedTime + 5;
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 5f, _playerRB.position.y, _playerRB.position.z));
        }

        if (collision.gameObject.tag == "BigMountain")
        {
            penguinPlayer.clip = collideObject;
            penguinPlayer.Play();

            //Debug.Log("vaya montaña loko");
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 15, _playerRB.position.y + 10, _playerRB.position.z - 5));
            obstacle = true;
            _timeSlow = Time.fixedTime + 2;
        }

        if (collision.gameObject.tag == "Cave")
        {
            penguinPlayer.clip = collideObject;
            penguinPlayer.Play();
            //Debug.Log("vaya montaña loko");
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 6, _playerRB.position.z - 10));
            obstacle = true;
            _timeSlow = Time.fixedTime + 2;
        }

        if (collision.gameObject.tag == "Ramp")
        {
            penguinPlayer.clip = dash;
            penguinPlayer.Play();

            onRamp = true;
            speed = 30;
        }

        if (collision.gameObject.tag == "BigRamp")
        {
            penguinPlayer.clip = dash;
            penguinPlayer.Play();

            onRamp = true;
            speed = 35;
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            penguinPlayer.clip = collideObject;
            penguinPlayer.Play();

            onObstacleMini = true;
            _timeObstacle = Time.fixedTime + 3;
            speed = 15;
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 3, _playerRB.position.y, _playerRB.position.z));
        }
    }

    #endregion

    #region HANDLER CONTROLLER

    //Asignamos las acciones mediante handlers
    private void OnEnable()
    {
        //Añade el evento moverse
        _controls.Player.ESC.performed += ESC;
        _controls.Player.Movement.performed += Move;
        //_controls.Player.Attack.performed += Attack; //Evento 
        _controls.Player.Run.performed += Jump; //Evento SALTAR
        _controls.Player.CameraControl.performed += GetCameraMove;//Movimiento de camara

        //Habilita el evento
        _controls.Player.Enable();
    }

    //Desactiva acciones
    private void OnDisable()
    {
        //Elimina el evento
        _controls.Player.ESC.canceled -= ESC;
        _controls.Player.Movement.canceled -= Move;
        //_controls.Player.Attack.canceled -= Attack;
        _controls.Player.Run.canceled -= Jump;
        _controls.Player.CameraControl.canceled -= GetCameraMove;
        _controls.Player.Disable();

    }

    #endregion

    #region PLAYER ACTIONS

    public void ESC(InputAction.CallbackContext context)
    {
        Pause();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontaldirection = context.ReadValue<Vector2>();
    }

    /*public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("BOFETÓN");
        //tiene que impulsar a otro pingu
        //CAMBIAR ANIMACIÓN
        //isAttacking = true;
        //_timeAttacking = Time.fixedTime; //tiempo que estará activo el ataque
    }*/

    public void Jump(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        if (inFloor == true)
        {
            Debug.Log("SALTO");
            //_playerRB.AddForce(Vector3.up * 0, ForceMode.Force); //hace fuerza hacia abajo de forma que no vuele
            _playerRB.AddForce(Vector3.up * 20f, ForceMode.Impulse);
            inFloor = false;
        }
    }


    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
    }

    #endregion



    #region ACTIONS TIME

    public void Pause()
    {
        if(PauseButtonActivo == false)
        {
            PauseButton.SetActive(true);
            PauseButtonActivo = true;
        }
        else
        {
            PauseButton.SetActive(false);
            PauseButtonActivo = false;
        }
    }


    public void ShowRanking()
    {
        //si todos han llegado

        if (Time.fixedTime > timeRanking)
        {
            finalText.SetActive(false);
            tableRanking.SetActive(true);
            HighscoreTable table = FindObjectOfType<HighscoreTable>();
            table.SetPlayerNames(matchInfo.clasification);
            table.ActualizeClasification();
        }

    }

    //Tiempo que dura con la velocidad por el pez
    public void FishRun(double deltaTime)
    {
        if (fishEaten == true)
        {
            if (deltaTime > _timeFish)
            {
                speed = 25;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeFish);
                fishEaten = false;
            }

        }
    }

    public void WasHitted(bool side) //false = izq, true = dch
    {

        if (isHitted == true)
        {
            Debug.Log("fuiste pegado");

            if (side == false)
            {
                Debug.Log("LEFT");
                //_playerRB.AddForce(Vector3.left * 250, ForceMode.Impulse);
                _playerRB.AddRelativeForce(Vector3.left * 250, ForceMode.Impulse);
                //penguinPos = _playerRB.position;
                //_playerRB.MovePosition(new Vector3(penguinPos.x, penguinPos.y, penguinPos.z - 5));
                isHitted = false;
            }
            else
            {
                Debug.Log("TRUE");
                _playerRB.AddRelativeForce(Vector3.right * 250, ForceMode.Impulse);
                isHitted = false;
            }
        }
    }


    #endregion

    #region CAMERA CONTROL

    //Ubica la direccion de la camara para que el personaje se mueva respecto a ella
    public void CameraDirection()
    {
        camFordward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;

        camFordward.y = 0; //No interesa tocar la componente y (arriba abjo)
        camRight.y = 0;

        //Da el valor normalizado de las camaras para poder ubicarlo mejor
        camFordward = camFordward.normalized;
        camRight = camRight.normalized;
    }


    //Controla la cámara que sigue al personaje
    public void ThirdCamera()
    {
        //Seguir al target == camara
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position, Time.deltaTime * 100);
        //Rotar la cámara
        mainCamera.transform.rotation = target.rotation;

        //Entrada del movimiento
        moveX += movementCamera.x * m_LookSense;
        moveY += movementCamera.y * m_LookSense;

        //limitar movimiento y entre -50 y 70
        moveY = Mathf.Clamp(moveY, -30.0f, 00.0f);

        //pivot sigue a player
        Vector3 follow = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 3f);
        pivot.position = Vector3.Lerp(pivot.position, follow, Time.deltaTime * 100);

        //Rotar pivot
        pivot.rotation = Quaternion.Euler(-moveY, moveX, 0.0f);
        //target.rotation = Quaternion.Euler(0,0, 0.0f);

    }

    #endregion

}
