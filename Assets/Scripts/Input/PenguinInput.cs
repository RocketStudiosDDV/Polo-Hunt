using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenguinInput : MonoBehaviour
{
    #region VARIABLES

    //PINGUINO HUD 
   // public GameObject BearHUD;
    //public GameObject PenguinHUD;

    //CONTROL DEL PLAYER
    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 3;
    [SerializeField] float maxSpeed = 10;

    private PlayerControls _controls;
    private Rigidbody _playerRB; //Rigid body del pingu
    private GameObject penguinBody;

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
    private float m_LookSense = 1.0f;

    private double _timeRunning;
    private bool isRunning = false;
    private Vector3 forceDirection;

    private bool cepoActive = false;
    private double timeAwait;

    private bool fishEaten = false;
    private double _timeFish;


    public GameObject cepo;

    private bool InIceDashPlat = false;

    //prueba bofetada 
    private bool caer = false;
    private bool tirar = false;
    private float _timeFall;
    private bool isAttacking = false;

    private MatchInfo matchInfo;

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        //BearHUD.SetActive(false);
        //PenguinHUD.SetActive(true);
        _playerRB = GetComponent<Rigidbody>();
        penguinBody = GetComponent<GameObject>();
        matchInfo = FindObjectOfType<MatchInfo>(); //si muere llamar a matchInfo.SpectatorMode
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerInput = new Vector3(_horizontaldirection.x, 0, _horizontaldirection.y);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      


        //Para que no supere una velocidad:
        /*if(_playerRB.velocity.magnitude > maxSpeed)
        {
            _playerRB.velocity = _playerRB.velocity.normalized * maxSpeed;
        }*/

        playerDirection = playerInput.x * camRight + playerInput.z * camFordward; //Almacena la direccion hacia la que se esta moviendo el player

        _playerRB.transform.LookAt(_playerRB.transform.position + playerDirection);

        if (isRunning == false)
        {
            CameraDirection();
        }

        ThirdCamera();


        target.transform.LookAt(pivot);

        ToRun(Time.fixedTime); //Comprueba si sigue deslizandose o no
        FishRun(Time.fixedTime); //Velocidad despues de comer el pez
        ToStand(Time.fixedTime);
        ToThrowStocks(Time.fixedTime);

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
            Debug.Log("direccion z " + playerDirection.z);
            Debug.Log("direccion x " + playerDirection.x);
            Debug.Log("VELOCIDAD " + _playerRB.velocity.magnitude);
            int lastPressed = 1;

            if (InIceDashPlat == true) //Movimientoi en el dash de la plat
            {
                _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);

                if (_playerRB.velocity.magnitude > 3)
                {
                    speed = speed / 2;
                }
                
                if (playerDirection.z > 0)
                {
                    _playerRB.AddForce(Vector3.forward * - 0.5f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.forward * 0.25f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.right * -0.25f * speed, ForceMode.Impulse);
                    lastPressed = 0;

                }
                else if (playerDirection.z < 0)
                {
                    _playerRB.AddForce(Vector3.forward * 0.5f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.forward * - 0.25f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.right * 0.25f * speed, ForceMode.Impulse);
                    lastPressed = 1;
                }
                else if(playerDirection.x > 0)
                {
                    _playerRB.AddForce(Vector3.left * 0.5f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.left * 0.25f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.forward * -0.25f * speed, ForceMode.Impulse);
                    lastPressed = 2;
                }
                else if (playerDirection.x < 0)
                {
                    _playerRB.AddForce(Vector3.right * 0.5f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.right * -0.25f * speed, ForceMode.Impulse);
                    _playerRB.AddForce(Vector3.forward * 0.25f * speed, ForceMode.Impulse);
                    lastPressed = 3;
                }
                else
                {
                    if(lastPressed == 0)
                    {
                        _playerRB.AddForce(Vector3.forward * 0.5f * speed, ForceMode.Impulse);
                        _playerRB.AddForce(Vector3.right * 0.25f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 1)
                    {
                        _playerRB.AddForce(Vector3.forward * -0.5f * speed, ForceMode.Impulse);
                        _playerRB.AddForce(Vector3.right * -0.25f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 2)
                    {
                        _playerRB.AddForce(Vector3.forward * 0.25f * speed, ForceMode.Impulse);
                        _playerRB.AddForce(Vector3.right * 0.5f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 3)
                    {
                        _playerRB.AddForce(Vector3.forward * -0.25f * speed, ForceMode.Impulse);
                        _playerRB.AddForce(Vector3.right * -0.5f * speed, ForceMode.Impulse);
                    }

                }

                //_playerRB.AddForce(Vector3.forward * - 0.25f * speed, ForceMode.Impulse);
                //Debug.Log("HIELO");
                isRunning = false;
            }
            else //movimiento normal
            {
                _playerRB.AddForce(Vector3.forward * 0, ForceMode.Impulse);
                _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Fish") //Si choca con un pescao
        {
            fishEaten = true;
            speed = 50;
            _timeFish = Time.fixedTime + 5;
            Debug.Log("COLAS");
        }

        if (collision.gameObject.tag == "IceDashPlat") //Si choca con un pescao
        {
            InIceDashPlat = true;
            Debug.Log("HIELO");

            if (isRunning == true) //Si esta corriendo deja de correr
            {
                isRunning = false;
                speed = 3;
            }
        }

        if (collision.gameObject.tag == "Floor") //Si choca con un pescao
        {

            Debug.Log("SUELO");
            //_playerRB.AddForce(Vector3.forward * 0, ForceMode.Impulse);
            //_playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);
            speed = 3;
            InIceDashPlat = false;
        }

        if (collision.gameObject.tag == "Penguin") //Si choca con un pinguino
        {
            if (isAttacking == true) //si estas dando colleja
            {
                //_timeFall = Time.fixedTime + 3;
                //Rigidbody enemy = collision.gameObject.GetComponent<Rigidbody>(); //le tiras
                //enemy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

                //LE MANDAS AL OTRO A CAERSE -> EJECUTAR TO FALL
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
    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("BOFETÓN");
        //CAMBIAR ANIMACIÓN
        //AÑADIR LO Q SEA PARA LA BOFETADA
        //tirar = true;
        isAttacking = true;
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("DESLIZA");
        forceDirection = _playerRB.transform.forward;
        isRunning = true;
        speed = 10;
        _timeRunning = Time.fixedTime + 10;
        Debug.Log("vel " + speed);
        //CAMBIAR ANIMACIÓN
    }

    public void PowerUp(InputAction.CallbackContext context) //Soltar cepo -L1 - space
    {
        //SOLTAR CEPO
        Debug.Log("CEPO");

        if (cepoActive == false)
        {
            cepoActive = true;
            Instantiate(cepo, _playerRB.transform.position, Quaternion.identity);
            timeAwait = Time.fixedTime + 10; //tiempo que tarada en voilver a tener aviable el cpeo
        }
    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
    }

    #endregion



    #region ACTIONS TIME

    //Controla el tiempo que esta activo el deslizamiento
    public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            Debug.Log("a correr");
           // Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo a correr " + _timeRunning);
            //_controls.Player.Movement.Disable();
            //playerInput = new Vector3(playerInput.x, 0, playerInput.z);
            //_playerRB.velocity = new Vector3(playerInput.x * 0.5f * speed, _playerRB.velocity.y, playerDirection.z * 0.5f * speed);
            _playerRB.AddForce(forceDirection * 10, ForceMode.Acceleration);
            //_playerRB.AddForce(playerInput * 0.25f, ForceMode.Impulse);

            if (_playerRB.velocity.magnitude < 1)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                Debug.Log("ESTOY");
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                //_controls.Player.Movement.Enable();
                isRunning = false;
            } else if (deltaTime > _timeRunning)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                Debug.Log("ESTOY");
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                //_controls.Player.Movement.Enable();
                isRunning = false;
            }
        }
    }

    //Tiempo que dura con la velocidad por el pez
    public void FishRun(double deltaTime)
    {
        if (fishEaten == true)
        {
            Debug.Log("a correr");
            Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo pasado " + _timeFish);

            if (deltaTime > _timeFish)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeFish);
                fishEaten = false;
            }

        }
    }

    //Se levanta despues de x tiempo caído
    public void ToStand (double deltaTime)
    {
        if (_playerRB.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
        {
            if (deltaTime > _timeFall)
            {
                _playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }


    //Caerse
    public void ToFall()
    {
        _timeFall = Time.fixedTime + 3;
        _playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }

    //morir
    public void ToDie()
    {
        _controls.Player.Disable();
        Destroy(gameObject);
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
        moveY = Mathf.Clamp(moveY, -50.0f, 70.0f);

        //pivot sigue a player
        Vector3 follow = new Vector3(this.transform.position.x, this.transform.position.y /*- 1.0f*/, this.transform.position.z);
        pivot.position = Vector3.Lerp(pivot.position, follow /*+ offset*/, Time.deltaTime * 100);

        //Rotar pivot
        pivot.rotation = Quaternion.Euler(-moveY, moveX, 0.0f);
        //target.rotation = Quaternion.Euler(0,0, 0.0f);

    }

    #endregion

}
