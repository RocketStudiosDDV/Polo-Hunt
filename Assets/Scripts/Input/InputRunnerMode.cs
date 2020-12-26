using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRunnerMode : MonoBehaviour
{
    #region VARIABLES

    //CONTROL DEL PLAYER
    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 3;
    [SerializeField] float maxSpeed = 10;

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
    private float m_LookSense = 1.0f;

    private bool fishEaten = false;
    private double _timeFish;

    //prueba bofetada 
    private bool caer = false;
    private bool tirar = false;
    private float _timeFall;

    
    //empujon
    private double _timeAttacking;
    private bool isAttacking = false;

    private bool toStop = false;
    private double _timeTillStop;

    public Vector3 penguinPos; //guarda la posicion por si el pingu se cae para respawnearlo ahi
    public bool inFloor = true; //comprueba si esta en el suelo o se ha caído

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
        _playerRB = GetComponent<Rigidbody>();
        matchInfo = FindObjectOfType<MatchInfo>(); //si muere llamar a matchInfo.SpectatorMode
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerInput = new Vector3(1, 0, _horizontaldirection.x);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      


        //Para que no supere una velocidad:
        /*if(_playerRB.velocity.magnitude > maxSpeed)
        {
            _playerRB.velocity = _playerRB.velocity.normalized * maxSpeed;
        }*/

        //playerDirection = playerInput.x * camRight + playerInput.z * camFordward; //Almacena la direccion hacia la que se esta moviendo el player

        _playerRB.transform.LookAt(_playerRB.transform.position + playerInput);

        CameraDirection();
        ThirdCamera();


        target.transform.LookAt(pivot);

        //ToRun(Time.fixedTime); //Comprueba si sigue deslizandose o no
        FishRun(Time.fixedTime); //Velocidad despues de comer el pez
        ToStand(Time.fixedTime);
        //ToThrowStocks(Time.fixedTime);


        //movimiento
        if (toStop == false)
        {
            _playerRB.AddForce(Vector3.left * 10, ForceMode.Impulse);
            _playerRB.velocity = new Vector3(playerInput.x * speed, _playerRB.velocity.y, playerInput.z * speed);
        }
        else
        {
            
            if (Time.fixedTime < _timeTillStop)
            {
                _playerRB.AddForce(Vector3.left * 10, ForceMode.Impulse);
                _playerRB.velocity = new Vector3(playerInput.x * speed, _playerRB.velocity.y, playerInput.z * speed);
            }
        }
        
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "RightSky") //Si esta en el mapa
        {
            inFloor = false;
            penguinPos = _playerRB.position;
            _playerRB.MovePosition(new Vector3(penguinPos.x, penguinPos.y + 10, penguinPos.z - 10)); //la coloca en medio, si la cuesta es recta no hace falta dcha izq, sino si
            Debug.Log("ME CAIGO");
        }

        if (collision.gameObject.tag == "LeftSky") //Si esta en el mapa
        {
            inFloor = false;
            penguinPos = _playerRB.position;
            _playerRB.MovePosition(new Vector3(penguinPos.x, penguinPos.y + 10, penguinPos.z + 10)); //la coloca en medio, si la cuesta es recta no hace falta dcha izq, sino si
            Debug.Log("ME CAIGO");
        }

        if (collision.gameObject.tag == "Goal") //Si esta en el mapa
        {
            Debug.Log("Oleee");
        }

        if (collision.gameObject.tag == "LeftPenguin") //Si choca con un pescao
        {

            WasHitted(false);
        }

        if (collision.gameObject.tag == "RightPenguin") //Si choca con un pescao
        {

            WasHitted(true);
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

        if (collision.gameObject.tag == "RestZone") //Si choca con un pescao
        {
            Debug.Log("DESCANSO");
            int numRand = Random.Range(1, 4);
            Debug.Log(numRand);
            _timeTillStop = Time.fixedTime + numRand;
            toStop = true;
            
        }

        if (collision.gameObject.tag == "Floor") //Si esta en el mapa
        {

            Debug.Log("SUELO");
            speed = 5;
            
            inFloor = true;
            //InIceDashPlat = false;
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
        if (collision.gameObject.tag == "caca") //Si choca con un pescao
        {

            ToDie();
            //
            mainCamera = collision.gameObject.GetComponent<Camera>();
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
        //_controls.Player.PowerUp.performed += PowerUp; //Evento power up
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
        //_controls.Player.PowerUp.canceled -= PowerUp;
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
        //tiene que impulsar a otro pingu
        //CAMBIAR ANIMACIÓN
        //AÑADIR LO Q SEA PARA LA BOFETADA
        //tirar = true;
        isAttacking = true;
        _timeAttacking = Time.fixedTime; //tiempo que estará activo el ataque
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("DESLIZA");
        //forceDirection = _playerRB.transform.forward;
        //isRunning = true;
        speed = 10;
        //_timeRunning = Time.fixedTime + 10;
        Debug.Log("vel " + speed);
        //CAMBIAR ANIMACIÓN
    }


    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
    }

    #endregion



    #region ACTIONS TIME

    //Controla el tiempo que esta activo el deslizamiento
    /*public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            Debug.Log("a correr");
            Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo pasado " + _timeRunning);
            //_controls.Player.Movement.Disable();
            //playerInput = new Vector3(playerInput.x, 0, playerInput.z);
            //_playerRB.velocity = new Vector3(playerInput.x * 0.5f * speed, _playerRB.velocity.y, playerDirection.z * 0.5f * speed);
            _playerRB.AddForce(forceDirection * speed, ForceMode.Acceleration);
            //_playerRB.AddForce(playerInput * 0.25f, ForceMode.Impulse);

            if (deltaTime > _timeRunning)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                //_controls.Player.Movement.Enable();
                isRunning = false;
            }
        }
    }*/

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
    public void ToStand(double deltaTime)
    {
        if (_playerRB.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
        {
            if (deltaTime > _timeFall)
            {
                _playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    public void WasHitted(bool side) //false = izq, true = dch
    {
        Debug.Log("fuiste pegado");

        if (side == false)
        {
            Debug.Log("LEFT");
            _playerRB.AddForce(Vector3.forward * -100, ForceMode.Impulse);
            //penguinPos = _playerRB.position;
            //_playerRB.MovePosition(new Vector3(penguinPos.x, penguinPos.y, penguinPos.z - 5));
        }
        else
        {
            Debug.Log("TRUE");
            _playerRB.AddForce(Vector3.forward * 100, ForceMode.Impulse);
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
    /*
    //Comprueba que puede tirar el cepo
    public void ToThrowStocks(double deltaTime)
    {
        if (cepoActive == true)
        {
            if (Time.fixedTime > timeAwait)
            {
                cepoActive = false;
            }
        }
    }*/

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
