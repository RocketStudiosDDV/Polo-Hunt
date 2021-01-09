using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenguinInputMultiplayer : MonoBehaviour
{
    #region VARIABLES

    //PINGUINO HUD 
   // public GameObject BearHUD;
    //public GameObject PenguinHUD;

    //CONTROL DEL PLAYER
    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 5;
    [SerializeField] float maxSpeed = 10;

    private PlayerControls _controls;
    private Rigidbody _playerRB; //Rigid body del pingu

    private Vector2 _horizontaldirection;
    private Vector3 playerInput; //guarda la info del input
    private Vector3 playerDirection; //Hacia donde se mueve el jugador

    //CONTROL DE LA CÁMARA
    public GameObject pivotPrefab;  // Prefab del pivot para instanciarlo

    public Camera mainCamera; //Guarda cuál es nuestra cámara
    private Vector3 camFordward; //vector camara hacia delante
    private Vector3 camRight; //camara hacia la dcha, direccion a la que mira la camara

    public Vector3 offset;
    public Vector2 movementCamera;
    public Transform target;
    public Transform pivot;
    private float moveX;
    private float moveY;
    private float m_LookSense = 1.0f;

    private double _timeRunning;
    private bool isRunning = false;
    private Vector3 forceDirection;

    private bool cepoActive = false;
    private double timeAwait;

    private bool fishEaten = false;
    private double _timeFish;

    public Vector3 lookingAt;
    public GameObject cepo;

    private bool InIceDashPlat = false;

    //CONTROL DE ANIMACIONES
    Animator penguin_animator;

    private bool walking_animation = false;
    private bool hit_animation = false;
    private bool sliding_animation = false;
    private bool afk_animation = false;


    //Variables bofetada
    private bool toFall = false;
    private bool enableControlsAfterFallen = true;
    //private bool tirar = false;
    private float _timeFall;
    private bool isAttacking = false;
    public double _timeAttacking;

    private MatchInfo matchInfo;

    // ONLINE
    private float maxDistanceToKill = 1f; // Distancia máxima entre el pingüino de los dos clientes para considerar que hubo kill
    private float maxPingToKill = 1000; // máximo ping para hacer la comprobación de distancia
    public int ownerActorNumber;
    private bool isDead = false;

    //CONTROL TECLADO
    public int keysPressed = 0; //control del movimiento por teclado para que no se acaben las animacione spor soltar una tecla al estar pulsando dos

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls();

        if (GetComponent<PhotonView>().IsMine || !PhotonNetwork.IsConnected)  // Si es nuestro pingüino, seguirlo con la cámara
        {
            mainCamera = Object.FindObjectOfType<Camera>();
            pivot = Instantiate(pivotPrefab).transform;
            target = pivot.GetChild(0).transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //BearHUD.SetActive(false);
        //PenguinHUD.SetActive(true);
        _playerRB = GetComponent<Rigidbody>();
        matchInfo = FindObjectOfType<MatchInfo>(); //si muere llamar a matchInfo.SpectatorMode

        penguin_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        //ANIMACIÓN ANDAR
        if (keysPressed > 0)
        {
            walking_animation = true;
        }
        else
        {
            walking_animation = false;
        }

        if (penguin_animator != null)
        {
            penguin_animator.SetBool("walking", walking_animation);
        }
            
        //penguin_animator.SetBool("sliding", sliding_animation);

        //ANIMACIÓN ATACAR
        if (isAttacking == false)
        {
            hit_animation = false;
        }
        else
        {
            hit_animation = true;
        }

        if (penguin_animator != null)
        {
            penguin_animator.SetBool("hit", hit_animation);
        }

        //ANIMACIÓN DESLIZARSE
        

        if (penguin_animator != null)
        {
            if (isRunning == true)
            {
                Debug.Log("running true true true");
                penguin_animator.SetBool("sliding", true);
            }
            else
            {
                penguin_animator.SetBool("sliding", false);
            }
        }

        //Caerse
        if (penguin_animator != null)
        {
            if (toFall == true)
            {
                penguin_animator.SetBool("sliding", true);
            }
        }

    }

    private void FixedUpdate()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        playerInput = new Vector3(_horizontaldirection.x, 0, _horizontaldirection.y);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      

        playerDirection = playerInput.x * camRight + playerInput.z * camFordward; //Almacena la direccion hacia la que se esta moviendo el player

        _playerRB.transform.LookAt(_playerRB.transform.position + playerDirection);
        lookingAt = _playerRB.transform.forward;

        if ((isRunning == false) && (_playerRB.transform.position.y > 133))
        {
            CameraDirection();
        }

        ThirdCamera();

        if (_playerRB.transform.position.y > 133)
        {
            ThirdCamera(); //control cámara tercera persona
        }


        target.transform.LookAt(pivot);

        ToRun(Time.fixedTime); //Comprueba si sigue deslizandose o no
        FishRun(Time.fixedTime); //Velocidad despues de comer el pez
        ToStand(Time.fixedTime);
        ToThrowStocks(Time.fixedTime);
        ToHit(Time.fixedTime); //Se asegura de que el ataque dura solo un instante (1 seg) y no que cuando le des este smp atacando

        /*if (caer == true)
        {
            _playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            if (Time.fixedTime > _timeFall)
            {
                _playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                caer = false;
            }
        }*/

        if (isRunning == false)
        {
            int lastPressed = 1;
            float indexBig = 0.3f;

            if (InIceDashPlat == true) //Movimientoi en el dash de la plat
            {
                _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);

                if (playerDirection.z > 0)
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    lastPressed = 0;

                }
                else if (playerDirection.z < 0)
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    lastPressed = 1;
                }
                else if(playerDirection.x > 0)
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    lastPressed = 2;
                }
                else if (playerDirection.x < 0)
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    lastPressed = 3;
                }
                else
                {
                    if(lastPressed == 0)
                    {
                        _playerRB.AddForce(lookingAt * 0.5f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 1)
                    {
                        _playerRB.AddForce(lookingAt * -0.5f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 2)
                    {
                        _playerRB.AddForce(lookingAt * 0.5f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 3)
                    {
                        _playerRB.AddForce(lookingAt * -0.5f * speed, ForceMode.Impulse);
                    }
                }

                isRunning = false;
            }
            else //movimiento normal
            {
                _playerRB.AddForce(Vector3.forward * 0, ForceMode.Impulse);
                _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);
            }
        }
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (collision.gameObject.tag == "Corner1")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 15, _playerRB.position.y + 20, _playerRB.position.z - 15));
        }

        if (collision.gameObject.tag == "Corner2")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 20, _playerRB.position.z - 10));
        }

        if (collision.gameObject.tag == "Corner3")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 20, _playerRB.position.z + 10));
        }

        if (collision.gameObject.tag == "Corner4")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 20, _playerRB.position.z + 10));
        }


        if (collision.gameObject.tag == "Side1")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x, _playerRB.position.y + 25, _playerRB.position.z - 10));
        }

        if (collision.gameObject.tag == "Side2")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x, _playerRB.position.y + 25, _playerRB.position.z + 10));
        }

        if (collision.gameObject.tag == "Side3")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 25, _playerRB.position.z));
        }

        if (collision.gameObject.tag == "Side4")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 25, _playerRB.position.z));
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
            speed = 4;
            _timeFish = Time.fixedTime + 5;
        }

        if (collision.gameObject.tag == "IceDashPlat") //Si choca con un pescao
        {
            InIceDashPlat = true;

            if (isRunning == true) //Si esta corriendo deja de correr
            {
                isRunning = false;
                speed = 3;
            }
        }

        if (collision.gameObject.tag == "Floor") //Si choca con un pescao
        {

            speed = 3;
            InIceDashPlat = false;
        }

        if (collision.gameObject.tag == "Penguin") //Si choca con un pinguino
        {
            if (isAttacking == true) //si estas dando colleja
            {
                //LE MANDAS AL OTRO A CAERSE -> EJECUTAR TO FALL
                collision.gameObject.GetComponent<PhotonView>().RPC("ToFall", collision.gameObject.GetComponent<PhotonView>().Owner);
                isAttacking = false;
            }
        }

        //PRUEBAS
      /*  if (collision.gameObject.tag == "caca") //Si choca con un pescao
        {

            ToDie();
            //
            mainCamera = collision.gameObject.GetComponent<Camera>();
        }*/
    }
    #endregion

    #region HANDLER CONTROLLER

    //Asignamos las acciones mediante handlers
    private void OnEnable()
    {
        //Añade el evento moverse
        _controls.Player.Movement.performed += Move;
        _controls.Player.Attack.performed += Attack; //Evento atacar
        _controls.Player.Run.performed += Run; //Evento correr
        _controls.Player.PowerUp.performed += PowerUp; //Evento power up
        _controls.Player.CameraControl.performed += GetCameraMove;//Movimiento de camara

        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        //Habilita el evento
        _controls.Player.Enable();
    }

    //Desactiva acciones
    private void OnDisable()
    {
        //Elimina el evento
        _controls.Player.Movement.canceled -= Move;
        _controls.Player.Attack.canceled -= Attack;
        _controls.Player.Run.canceled -= Run;
        _controls.Player.PowerUp.canceled -= PowerUp;
        _controls.Player.CameraControl.canceled -= GetCameraMove;
        _controls.Player.Disable();

    }

    #endregion

    #region PLAYER ACTIONS
    public void Move(InputAction.CallbackContext context)
    {
        _horizontaldirection = context.ReadValue<Vector2>();

        if (context.control.IsPressed() == true)
        {
            keysPressed++;
        }
        else
        {
            keysPressed--;
        }
    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        isAttacking = true;
        _timeAttacking = Time.fixedTime + 2;
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        forceDirection = _playerRB.transform.forward; //direccion en la que se va a deslizar
        isRunning = true;
        speed = 30;
        _timeRunning = Time.fixedTime + 3;
        _controls.Player.Run.Disable();
        _controls.Player.Movement.Disable();
    }

    public void PowerUp(InputAction.CallbackContext context) //Soltar cepo -L1 - space
    {
        //SOLTAR CEPO
        if (cepoActive == false)
        {
            cepoActive = true;

            if ((lookingAt.x > 0) && (lookingAt.z < 0))
            {
                PhotonNetwork.Instantiate(this.cepo.name, new Vector3(_playerRB.transform.position.x, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z + 1), Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x < 0) && (lookingAt.z < 0))
            {
                PhotonNetwork.Instantiate(this.cepo.name, new Vector3(_playerRB.transform.position.x + 1, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z), Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x > 0) && (lookingAt.z > 0))
            {
                PhotonNetwork.Instantiate(this.cepo.name, new Vector3(_playerRB.transform.position.x - 1, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z), Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x < 0) && (lookingAt.z > 0))
            {
                PhotonNetwork.Instantiate(this.cepo.name, new Vector3(_playerRB.transform.position.x, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z - 1), Quaternion.Euler(-90f, 0f, 0f));
            }

            timeAwait = Time.fixedTime + 5; //tiempo que tarada en voilver a tener aviable el cpeo
        }
    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        if (isRunning == true)
        {
            movementCamera = context.ReadValue<Vector2>() / 2; //ralentiza el movimiento de la camara site estas deslizando
        }
        else
        {
            movementCamera = context.ReadValue<Vector2>();
        }
    }

    #endregion



    #region ACTIONS TIME

    //Controla el tiempo que esta activo el deslizamiento
    public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            /*if ((deltaTime > _timeRunning - 3) && (deltaTime < _timeRunning - 2))
            {
                _playerRB.AddForce(forceDirection * 7, ForceMode.Acceleration);
                //Debug.Log("3 segs");
            }
            else */if ((deltaTime > _timeRunning - 2) && (deltaTime < _timeRunning - 1))
            {
                _playerRB.AddForce(forceDirection * 5, ForceMode.Acceleration);
            }
            else if ((deltaTime > _timeRunning - 1) && (deltaTime < _timeRunning))
            {
                _playerRB.AddForce(forceDirection * 2, ForceMode.Acceleration);
            }
            else
            {
                _playerRB.AddForce(forceDirection * 10, ForceMode.Acceleration);
            }

            //FORMAS DE PARARSE
            if (_playerRB.velocity.magnitude < 1)
            {
                speed = 3;
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                isRunning = false;
            } 
            else if (deltaTime > _timeRunning)
            {
                speed = 3;
                _controls.Player.Movement.Disable();
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                isRunning = false;
            }
        }
        else
        {
            if (deltaTime > _timeRunning + 3)
            {
                speed = 5;
                if (enableControlsAfterFallen == true)
                {
                    _controls.Player.Movement.Enable();
                    _controls.Player.Run.Enable();
                }
            }
        }
    }

    //Tiempo que dura con la velocidad por el pez
    public void FishRun(double deltaTime)
    {
        if (fishEaten == true)
        {
            if ((deltaTime > _timeFish - 4) && (deltaTime < _timeFish - 3))
            {
                speed = 6;
            }
            else if ((deltaTime > _timeFish - 3) && (deltaTime < _timeFish - 2))
            {
                speed = 8;
            }
            else if ((deltaTime > _timeFish - 2) && (deltaTime < _timeFish - 1))
            {
                speed = 10;
            }
            else if ((deltaTime > _timeFish - 1) && (deltaTime < _timeFish))
            {
                speed = 6;
            }

            if (deltaTime > _timeFish)
            {
                speed = 3;
                fishEaten = false;
            }

        }
    }

    //Se levanta despues de x tiempo caído
    public void ToStand (double deltaTime)
    {
        if (toFall == true)
        {
            if (deltaTime > _timeFall)
            {
                Debug.Log("ya estoy bien");
                toFall = false;
            }
        }

        if (enableControlsAfterFallen == false)
        {
            if (deltaTime > _timeFall + 1)
            {
                speed = 3;
                _controls.Player.Run.Enable();
                enableControlsAfterFallen = true;
            }
        }
    }


    //Caerse
    [PunRPC]
    public void ToFall()
    {
        _controls.Player.Movement.Disable();
        _controls.Player.Run.Disable();
        speed = 0;
        _timeFall = Time.fixedTime + 5;
        enableControlsAfterFallen = false;
        toFall = true;
    }

    //morir
    public void ToDie()
    {
        _controls.Player.Disable();
        Destroy(gameObject);
    }

    [PunRPC]
    public void ToDie(object[] objectArray)
    {
        if (isDead)
            return;
        isDead = true;
        if (PhotonNetwork.IsConnected)
        {
            Vector3 positionCheck = (Vector3) objectArray[0];
            int bearActorNumber = (int)objectArray[1];
            if (Vector3.Distance(positionCheck, transform.position) < maxDistanceToKill || PhotonNetwork.GetPing() > maxPingToKill)
            {
                ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                hashtable.Add("alive", false);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                GameObject killerBear = null;
                foreach(BearInputMultiplayer bear in FindObjectsOfType<BearInputMultiplayer>())
                {
                    if (bear.ownerActorNumber == bearActorNumber)
                        killerBear = bear.gameObject;
                }
                matchInfo.GetComponent<PhotonView>().RPC("ActualizeNumPenguins", RpcTarget.All);
                matchInfo.SpectatorMode(gameObject, killerBear);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    //Comprueba que puede tirar el cepo
    public void ToThrowStocks (double deltaTime)
    {
        if (cepoActive == true)
        {
            if (Time.fixedTime > timeAwait)
            {
                cepoActive = false;
            }
        }
    }

    public void ToHit(double deltaTime)
    {
        if (isAttacking == true)
        {
            if (Time.fixedTime > _timeAttacking)
            {
                isAttacking = false;
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
        moveY = Mathf.Clamp(moveY, -30.0f, 25.0f);

        //pivot sigue a player
        Vector3 follow = new Vector3(this.transform.position.x, this.transform.position.y /*- 1.0f*/, this.transform.position.z);
        pivot.position = Vector3.Lerp(pivot.position, follow /*+ offset*/, Time.deltaTime * 100);

        //Rotar pivot
        pivot.rotation = Quaternion.Euler(-moveY, moveX, 0.0f);
        //target.rotation = Quaternion.Euler(0,0, 0.0f);

    }

    #endregion

}
