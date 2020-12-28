using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

public class BearInputMultiplayer : MonoBehaviour
{
    #region VARIABLES
    //HUD OSO
    public GameObject BearHUD;
    public GameObject PenguinHUD;

    //CONTROL DEL PLAYER
    [SerializeField] float speed = 3;
    //[SerializeField] float jumpForce = 10;
    //[SerializeField] float maxSpeed = 10;

    private PlayerControls _controls; //Consigue los controles por teclado y mando
    private Rigidbody _playerRB; //Rigid body del pingu, controla las fisicas

    private Vector2 _horizontaldirection; //Vector que recoge la info del joystick de movimiento (direccion de mov  y cantidad)
    private Vector3 playerInput; //Guarda la info del input
    private Vector3 playerDirection; //Hacia donde se mueve el jugador

    //CONTROL DE LA CÁMARA
    public GameObject pivotPrefab;  // Prefab para instanciar el pivot

    //Gestión del movimiento del personaje respecto de la cámara
    public Camera mainCamera; //Guarda cuál es nuestra cámara
    private Vector3 camFordward; //vector camara hacia delante
    private Vector3 camRight; //camara hacia la dcha, direccion a la que mira la camara

    //Gestión de la cámara en tercera persona
    public Transform target;
    public Transform pivot;
    public Vector3 offset;
    public Vector2 movementCamera;
    private float moveX;
    private float moveY;
    private float m_LookSense = 1.0f;

    //Daño por caida al agua del oso
    private double _timeDamage;
    private bool damaged = false;

    //Gestión de la stamina
    private double finalStamina;
    private float stamina = 600;
    public Image Stamina;
    private bool firstTime = true;

    //Gestiona correr
    private bool isRunning = false;
    private double _timeRunning;

    //Gestiona el ataque
    private bool atacking = false;
    private double _timeAtacking;

    //Gestiona el power up
    private bool powerUpOn = false;
    private double _timePowerUp;

    public Material visionMaterial;
    public Material normalMaterial;

    

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls(); //Recoge los controles
        if (GetComponent<PhotonView>().IsMine || !PhotonNetwork.IsConnected)  // Si es nuestro pingüino, seguirlo con la cámara
        {
            mainCamera = Object.FindObjectOfType<Camera>();
            pivot = Instantiate(pivotPrefab).transform;
            target = pivot.GetChild(0).transform;
        }
    }

    void Start()
    {
        _playerRB = GetComponent<Rigidbody>(); //identifica el rigidbdy del oso
        BearHUD.SetActive(true);
        PenguinHUD.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        playerInput = new Vector3(_horizontaldirection.x, 0, _horizontaldirection.y);
        playerInput = Vector3.ClampMagnitude(playerInput, 1); //para poder normalizar la distancia. 1 es el valor max, va de 0 a 1      

        //Para que no supere una velocidad:
        /*if(_playerRB.velocity.magnitude > maxSpeed)
        {
            _playerRB.velocity = _playerRB.velocity.normalized * maxSpeed;
        }*/

        playerDirection = playerInput.x * camRight + playerInput.z * camFordward; //Almacena la direccion hacia la que se esta moviendo el player

        _playerRB.transform.LookAt(_playerRB.transform.position + playerDirection); //Hace que el jugador mire al frente

        //Funciones de control de cámara
        CameraDirection(); //movimiento respecto de la camara
        ThirdCamera(); //control cámara tercera persona

        target.transform.LookAt(pivot);

        //Aplicamos la velocidad de movimiento WASD
        _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);

        ToDamage(Time.fixedTime);
        ToRun(Time.fixedTime);
        UsePoweUp(Time.fixedTime);
        ToAttack(Time.fixedTime);
        

        if (_playerRB.transform.position.y < -1.25)
        {
            _playerRB.MovePosition(new Vector3(_playerRB.transform.position.x + 3.0f, 3.0f, _playerRB.transform.position.z));
        }

        //Debug.Log(_playerRB.velocity.magnitude);
    }

    private void LateUpdate()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        Stamina.fillAmount = stamina/600;
        //transform.LookAt(_playerRB.transform.position);
        //pivot.LookAt(target.position);
        //pivot.LookAt(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (collision.gameObject.tag == "Stocks") //Si choca con un cepo
        {
            //Hacer daño o lo q sea
            _timeDamage = Time.fixedTime + 10;
            Debug.Log("DAÑO");
            speed = 0;
            _controls.Player.Movement.Disable();
            _controls.Player.CameraControl.Disable();
            damaged = true;
        }

        if (collision.gameObject.tag == "Fish") //Si choca con un cepo
        {
            stamina += 200;
            Debug.Log("COLAS");
        }

        if (collision.gameObject.tag == "Penguin")
        {
            if (atacking == true)
            {
                GameObject pengu = collision.gameObject;

                Destroy(pengu);
            }
        }
    }

    #endregion

    #region HANDLER INPUTS
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

    //Desactivamos la acción
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

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //CAMBIAR ANIMACIÓN
        //AÑADIR LO Q SEA PARA EL ZARPAZO
        atacking = true;
        _timeAtacking = Time.fixedTime + 1;
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //CAMBIAR ANIMACIÓN
        //AÑADIR VELOCIDAD

        if (firstTime == true)
        {
            stamina += Time.fixedTime;
            firstTime = false;
        }

        isRunning = true;
        speed = 10;
        _timeRunning = Time.fixedTime + 10;
        finalStamina = Time.fixedTime;


        //Stamina = 10/Time.deltaTime/1000;
        //Stamina += /*10 */ Time.deltaTime; /// 1000;
        //Debug.Log("Stamina " + Stamina);
    }
    public void PowerUp(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //ACTIVAR VISIÓN
        GameObject [] penguins = GameObject.FindGameObjectsWithTag("Penguin");

        for (int i = 0; i < penguins.Length; i++)
        {
            Renderer rend = penguins[i].GetComponent<Renderer>();
            rend.material = visionMaterial;
        }

        powerUpOn = true;
        _timePowerUp = Time.fixedTime + 2;
}

    public void Move(InputAction.CallbackContext context)
    {
        //Debug.Log(context.control.device.displayName);
        _horizontaldirection = context.ReadValue<Vector2>();
    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
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

    public void ThirdCamera()
    {
        //Seguir al target == camara
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position /*+ offset*/, Time.deltaTime * 100);
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

    //Que el daño dure x tiempo
    public void ToDamage(double deltaTime)
    {
        if (damaged == true)
        {
            if (deltaTime > _timeDamage)
            {
                speed = 3;
                _controls.Player.Movement.Enable();
                _controls.Player.CameraControl.Enable();
                damaged = false;
                Debug.Log("Ya estoy bien :)");
            }
        }
    }

    //Que el ataque dure un seg
    public void ToAttack(double deltaTime)
    {
        if (deltaTime > _timeAtacking)
        {
            atacking = false;
        }
    }

    //Tiempo que dura la vision
    public void UsePoweUp (double deltaTime)
    {
        if (powerUpOn == true)
        {
            if(deltaTime > _timePowerUp)
            {
                GameObject[] penguins = GameObject.FindGameObjectsWithTag("Penguin");

                for (int i = 0; i < penguins.Length; i++)
                {
                    Renderer rend = penguins[i].GetComponent<Renderer>();
                    rend.material = normalMaterial;
                }

                powerUpOn = false;
            }
        }
    }

    //Controla el tiempo que esta activo correr y la stamina
    public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            Debug.Log("a correr");
            Debug.Log("delta time " + deltaTime);
            Debug.Log("tiempo pasado " + _timeRunning);

            stamina--;
            
            Debug.Log("final stamina " + finalStamina);
            Debug.Log("stamina " + stamina);

            if (finalStamina > stamina)
            {
                speed = 3;
                Debug.Log("delta time " + Time.deltaTime);
                Debug.Log("tiempo pasado " + _timeRunning);
                isRunning = false;
            }

            //Stamina -= Time.deltaTime;
            //Debug.Log("STAMINA" + Stamina);

            /* if (deltaTime > _timeRunning)
             {
                 speed = 3;
                 Debug.Log("delta time " + Time.deltaTime);
                 Debug.Log("tiempo pasado " + _timeRunning);
                 isRunning = false;
             }*/

        }
        else
        {
            if (stamina < deltaTime + 600)
            {
                stamina++;
            }

        }
    }

}


