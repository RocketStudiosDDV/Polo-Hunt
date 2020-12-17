using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinguPruebaInput : MonoBehaviour
{
    #region VARIABLES

    //CONTROL DEL PLAYER
    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 3;
    [SerializeField] float maxSpeed = 10;

    private PlayerControls _controls;
    //private Rigidbody _playerRB; //Rigid body del pingu
    public CharacterController _player;
    public float gravity = 9.8f;

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

    private bool cepoActive = false;
    private double timeAwait;

    private bool fishEaten = false;
    private double _timeFish;


    public GameObject cepo;

    private bool InIceDashPlat = false;

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        //_playerRB = GetComponent<Rigidbody>();
        _player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerInput = new Vector3(_horizontaldirection.x, _player.velocity.y, _horizontaldirection.y);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      

        if (InIceDashPlat == true)
        {
            playerInput = new Vector3(_horizontaldirection.x * 0.1f, 0, _horizontaldirection.y * 0.1f);
            Debug.Log("HIELO");
        }
        //Para que no supere una velocidad:
        /*if(_playerRB.velocity.magnitude > maxSpeed)
        {
            _playerRB.velocity = _playerRB.velocity.normalized * maxSpeed;
        }*/

       
        _player.transform.LookAt(_player.transform.position + playerDirection);

        if (isRunning == false)
        {
            CameraDirection();
        }



        playerDirection = playerInput.x * camRight + playerInput.z * camFordward; //Almacena la direccion hacia la que se esta moviendo el player
        playerDirection = new Vector3(playerInput.x * speed, _player.velocity.y * playerInput.y, playerInput.z * speed);

        ThirdCamera();

        target.transform.LookAt(pivot);

        SetGravity(); //inicializa la gravedad

        //Aplicamos la velocidad de movimiento WASD
        //_player.Move(new Vector3(playerInput.x * speed, _player.velocity.y, playerInput.z * speed) * Time.deltaTime);
        _player.Move(new Vector3(playerInput.x, playerInput.y , playerInput.z ) * Time.deltaTime);
        //_playerRB.AddForce(Vector3.right * speed * _horizontaldirection);


        ToRun(Time.fixedTime); //Comprueba si sigue deslizandose o no
        FishRun(Time.fixedTime);

        //comprueba que puede tirar el cepo. COODOWN
        if (cepoActive == true)
        {
            if (Time.fixedTime > timeAwait)
            {
                cepoActive = false;
            }
        }
    }

    private void LateUpdate()
    {

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

        }
        else
        {
            InIceDashPlat = false;
        }

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
        //Debug.Log(context.control.device.displayName);
        //_player.Move(Vector3.up * jumpForce, ForceMode.Impulse);
        //_player.
        //CAMBIAR ANIMACIÓN
        //AÑADIR LO Q SEA PARA LA BOFETADA
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("DESLIZA");

        isRunning = true;
        speed = 10;
        _timeRunning = Time.fixedTime + 10;
        Debug.Log("vel " + speed);
        //CAMBIAR ANIMACIÓN

    }
    public void PowerUp(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //SOLTAR CEPO
        Debug.Log("CEPO");

        if (cepoActive == false)
        {
            cepoActive = true;
            Instantiate(cepo, _player.transform.position, Quaternion.identity);
            timeAwait = Time.fixedTime + 10; //tiempo que tarada en voilver a tener aviable el cpeo

        }

    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
    }

    #endregion

    //Controla el tiempo que esta activo el deslizamiento
    public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            Debug.Log("a correr");
            Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo pasado " + _timeRunning);
            _controls.Player.Movement.Disable();

            if (deltaTime > _timeRunning)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                _controls.Player.Movement.Enable();
                isRunning = false;
            }

        }
    }

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

    public void SetGravity() 
    {
        

        if (_player.isGrounded)
        {
            playerInput.y = -gravity * Time.deltaTime;
        }
        else
        {
            playerInput.y -= gravity * Time.deltaTime; //asi genera una aceñleracion de caida en el aire

        }
    }

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
