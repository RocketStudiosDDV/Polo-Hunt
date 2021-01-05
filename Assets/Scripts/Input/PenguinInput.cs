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

    private SkinnedMeshRenderer penguinSkin;

    private MatchInfo matchInfo;

    public int keysPressed = 0;

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
        penguin_animator = GetComponent<Animator>();
        matchInfo = FindObjectOfType<MatchInfo>(); //si muere llamar a matchInfo.SpectatorMode
        penguinSkin = FindObjectOfType<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        //ANIMACIÓN ANDAR

        if(keysPressed > 0)
        {
            walking_animation = true;
        }
        else{
            walking_animation = false;
        }
        penguin_animator.SetBool("walking", walking_animation);

        //ANIMACIÓN ATACAR
        if (isAttacking == false)
        {
            hit_animation = false;
        } 
        else
        {
            hit_animation = true;
        }

        penguin_animator.SetBool("hit", hit_animation);

        //ANIMACIÓN DESLIZARSE
        if (isRunning == true)
        {
            Debug.Log("running true true true");
            sliding_animation = true;
            penguin_animator.SetBool("sliding", true);
        }
        else
        {
            sliding_animation = false;
            penguin_animator.SetBool("sliding", false);
        }

       
        //CaerSe
        if (toFall == true)
        {
            penguin_animator.SetBool("sliding", true);
        }
        
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
        lookingAt = _playerRB.transform.forward;//(_playerRB.transform.position + playerDirection).normalized;

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
            Debug.Log("direccion z " + playerDirection.z);
            Debug.Log("direccion x " + playerDirection.x);
            Debug.Log("VELOCIDAD " + _playerRB.velocity.magnitude);
            int lastPressed = 1;
            float indexBig = 0.3f;
            //float indexSmall = 0.25f;

            if (InIceDashPlat == true) //Movimiento en el dash de la plat
            {
                _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);
                speed = 2;
                /*
                if (_playerRB.velocity.magnitude > 3)
                {
                    indexBig = 0.3f;
                    indexSmall = 0.15f;
                    speed = speed / 2;
                }
                else
                {
                    speed = 3;
                    indexBig = 0.4f;
                    indexSmall = 0.25f;
                }
                */
                if ((lookingAt.z > 0) && (lookingAt.x > 0))
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.forward * indexSmall * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.right * -indexSmall * speed, ForceMode.Impulse);
                    lastPressed = 0;

                }
                else if ((lookingAt.z < 0) && (lookingAt.x > 0))
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.right * -indexSmall * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.forward * indexSmall * speed, ForceMode.Impulse);
                    lastPressed = 1;
                }
                else if ((lookingAt.x < 0) && (lookingAt.z > 0))
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                   // _playerRB.AddForce(Vector3.right * indexSmall * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.forward * -indexSmall * speed, ForceMode.Impulse);
                    lastPressed = 2;
                }
                else if ((lookingAt.x < 0) && (lookingAt.z < 0))
                {
                    _playerRB.AddForce(lookingAt * -indexBig * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.forward * indexSmall * speed, ForceMode.Impulse);
                    //_playerRB.AddForce(Vector3.right * indexSmall * speed, ForceMode.Impulse);
                    lastPressed = 3;
                }
                else
                {
                    if(lastPressed == 0)
                    {
                        _playerRB.AddForce(lookingAt * 0.5f * speed, ForceMode.Impulse);
                        //_playerRB.AddForce(Vector3.right * 0.25f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 1)
                    {
                        _playerRB.AddForce(lookingAt * -0.5f * speed, ForceMode.Impulse);
                        //_playerRB.AddForce(Vector3.right * -0.25f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 2)
                    {
                        _playerRB.AddForce(lookingAt * 0.5f * speed, ForceMode.Impulse);
                        //_playerRB.AddForce(Vector3.right * 0.5f * speed, ForceMode.Impulse);
                    }
                    else if (lastPressed == 3)
                    {
                        _playerRB.AddForce(lookingAt * -0.5f * speed, ForceMode.Impulse);
                        //_playerRB.AddForce(Vector3.right * -0.5f * speed, ForceMode.Impulse);
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
            speed = 4;
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
            speed = 3;
            InIceDashPlat = false;
        }

        if (collision.gameObject.tag == "Penguin") //Si choca con un pinguino
        {
            /*if (isAttacking == true) //si estas dando colleja
            {
                //_timeFall = Time.fixedTime + 3;
                //Rigidbody enemy = collision.gameObject.GetComponent<Rigidbody>(); //le tiras
                //enemy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

                //LE MANDAS AL OTRO A CAERSE -> EJECUTAR TO FALL
                isAttacking = false;
            }*/

            ToFall();
            Destroy(collision.gameObject);
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

        //if (context.control.IsPressed() == true) context.control.IsActuated[0]

        if (context.control.IsPressed() == true)
        {
            //walking_animation = true;
            keysPressed++;
        }
        else
        {
            keysPressed--;
            //walking_animation = false;
        }
    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("BOFETÓN");

        //AÑADIR LO Q SEA PARA LA BOFETADA
        //tirar = true;
        isAttacking = true;
        _timeAttacking = Time.fixedTime + 2;
        //CAMBIAR ANIMACIÓN
        //hit_animation = true;
        //penguin_animator.SetBool("hit", hit_animation);
        //hit_animation = false;
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log("DESLIZA");
        forceDirection = _playerRB.transform.forward;
        isRunning = true;
        speed = 10;
        _timeRunning = Time.fixedTime + 3;
        _controls.Player.Run.Disable();
        _controls.Player.Movement.Disable();
        Debug.Log("vel " + speed);
    }

    public void PowerUp(InputAction.CallbackContext context) //Soltar cepo -L1 - space
    {
        //SOLTAR CEPO
        Debug.Log("CEPO");

        if (cepoActive == false)
        {
            cepoActive = true;

            if ((lookingAt.x > 0) && (lookingAt.z < 0))
            {
                Instantiate(cepo, new Vector3(_playerRB.transform.position.x, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z + 1),Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x < 0)&& (lookingAt.z < 0))
            {
                Instantiate(cepo, new Vector3(_playerRB.transform.position.x + 1, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z), Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x > 0) && (lookingAt.z > 0))
            {
                Instantiate(cepo, new Vector3(_playerRB.transform.position.x - 1, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z ), Quaternion.Euler(-90f, 0f, 0f));

            }
            else if ((lookingAt.x < 0) && (lookingAt.z > 0))
            {
                Instantiate(cepo, new Vector3(_playerRB.transform.position.x, _playerRB.transform.position.y + 0.04f, _playerRB.transform.position.z - 1), Quaternion.Euler(-90f, 0f, 0f));

            }
            //Instantiate(cepo, new Vector3 (_playerRB.transform.position.x , _playerRB.transform.position.y, _playerRB.transform.position.z - (lookingAt.normalized.z * 1)), Quaternion.identity);
            timeAwait = Time.fixedTime + 1; //tiempo que tarada en voilver a tener aviable el cpeo
        }
    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        if (isRunning == true)
        {
            movementCamera = context.ReadValue<Vector2>()/2;
        }
        else{
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
            Debug.Log("a correr");
            Debug.Log("tiempo a correr " + _timeRunning);
            Debug.Log("velocidad" + _playerRB.velocity.magnitude);
            

            /*if ((deltaTime > _timeRunning - 3) && (deltaTime < _timeRunning - 2))
            {
                _playerRB.AddForce(forceDirection * 7, ForceMode.Acceleration);
                Debug.Log("3 segs");
            }
            else*/ if ((deltaTime > _timeRunning - 2) && (deltaTime < _timeRunning - 1))
            {
                _playerRB.AddForce(forceDirection * 5, ForceMode.Acceleration);
                Debug.Log("2 segs");
            }
            else if ((deltaTime > _timeRunning - 1) && (deltaTime < _timeRunning))
            {
                _playerRB.AddForce(forceDirection * 2f, ForceMode.Acceleration);
                Debug.Log("1 segs");
            }
            else
            {
                _playerRB.AddForce(forceDirection * 10, ForceMode.Force);
                Debug.Log("0 segs");
            }

            //FORMAS DE PARARSE
            if (_playerRB.velocity.magnitude < 1)
            {
                speed = 0;
                //_controls.Player.Movement.Disable();
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                Debug.Log("ESTOY");
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                isRunning = false;
               // sliding_animation = false;
            } 
            else if (deltaTime > _timeRunning)
            {
                speed = 0;
                //_controls.Player.Movement.Disable();
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                Debug.Log("ESTOY");
                
                _playerRB.AddForce(forceDirection * 0, ForceMode.Acceleration);
                isRunning = false;
               // sliding_animation = false;                
            }
        }
        else
        {
            if (deltaTime > _timeRunning + 3)
            {
                Debug.Log("AWAKE");
                speed = 3;
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
            Debug.Log("a correr");
            Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo pasado " + _timeFish);

            if ((deltaTime > _timeFish - 4) && (deltaTime < _timeFish - 3))
            {
                speed = 6;
                Debug.Log("speed 6 " + speed);
            }
            else if ((deltaTime > _timeFish - 3) && (deltaTime < _timeFish - 2))
            {
                speed = 8;
                Debug.Log("speed 8 " + speed);

            }
            else if ((deltaTime > _timeFish - 2) && (deltaTime < _timeFish - 1))
            {
                speed = 10;
                Debug.Log("speed 10 " + speed);
            }
            else if ((deltaTime > _timeFish - 1) && (deltaTime < _timeFish))
            {
                speed = 6;
                Debug.Log("speed 10 " + speed);
            }/*
            else
            {
                speed = 4;
                Debug.Log("speed 6 " + speed);
            }*/

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
    public void ToFall()
    {
        Debug.Log("Me escoño");
        _controls.Player.Movement.Disable();
        _controls.Player.Run.Disable();
        speed = 0;
        _timeFall = Time.fixedTime + 2;
        //_playerRB.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        //sliding_animation = true;      
        enableControlsAfterFallen = false;
        toFall = true;
       
    }

    //morir
    public void ToDie()
    {
        _controls.Player.Disable();
        Destroy(gameObject);
    }

    public void ToHit (double deltaTime)
    {
        if (isAttacking == true)
        {
            if (Time.fixedTime > _timeAttacking)
            {
                isAttacking = false;
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
