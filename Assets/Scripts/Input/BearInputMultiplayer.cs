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

    //CONTROL MOVIMIENTOS

    public int keysPressed = 0;


    //Gestiona daño por caida al agua y cepo del oso
    private double _timeDamage;
    private bool damaged = false;

    //Gestiona la stamina
    private double finalStamina;
    private float stamina = 600;
    public Image Stamina;
    private bool firstTime = true;

    //Gestiona correr
    private bool isRunning = false;
    private double _timeRunning;

    //Gestiona el ataque
    private bool attacking = false;
    private double _timeAtacking;

    //Gestiona el power up
    private bool powerUpOn = false;
    private bool powerUpAviable = true;
    private double _timePowerUp;
    private double cooldowmPowerUp;

    private bool outOfMap = false;
    public bool keyboardRunning = false;
    //Materiales que controlan la vuision berserker

    //public Material visionMaterial;
    //public Material normalMaterial;

    public Material visionMaterial1;
    public Material visionMaterial2;
    public Material visionMaterial3;

    public Material normalMaterial1;
    public Material normalMaterial2;
    public Material normalMaterial3;

    //CONTROL DE ANIMACIONES
    Animator bear_animator;

    private bool walking_animation = false;
    private bool attacking_animation = false;
    private bool running_animation = false;
    private bool damage_animation = false;

    private bool afk_animation = false;

    // ONLINE
    public int ownerActorNumber;

    //VISION ACTIVACION HUD
    private GameObject visionHud;
    private Canvas canvasHUD;
    //public Transform canvasNameHUD;
    private bool isBear = false;

    public GameObject PauseButton;
    public bool PauseButtonActivo = false;

    //EFECTOS DE AUDIO
    private AudioSource bearPlayerEffects;
    private AudioSource bearPlayer;
    public AudioClip eatFishEffect;
    public AudioClip iceDashEffect;
    public AudioClip snowWalkEffect;
    public AudioClip damageStockEffect;
    public AudioClip crashIceEffect;
    public AudioClip fallInWater;
    public AudioClip penguinDeathEffect;

    private bool onFloor = false;
    private bool onIce = false;

    //BARRA STAMINA
    private float maxStamina = 600;
    private GameObject staminaBarObject;
    private Image staminaBar;
    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        _controls = new PlayerControls(); //Recoge los controles
        bear_animator = GetComponent<Animator>();
        if (GetComponent<PhotonView>().IsMine || !PhotonNetwork.IsConnected)  // Si es nuestro pingüino, seguirlo con la cámara
        {
            mainCamera = Object.FindObjectOfType<Camera>();
            pivot = Instantiate(pivotPrefab).transform;
            target = pivot.GetChild(0).transform;
        }

        foreach(Button b in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (b.gameObject.CompareTag("PAUSE"))
                PauseButton = b.gameObject;
        }
    }

    void Start()
    {
        _playerRB = GetComponent<Rigidbody>(); //identifica el rigidbdy del oso
        //bearPlayer = GetComponent<AudioSource>().CompareTag("Bear");

        foreach (AudioSource audiosource in Resources.FindObjectsOfTypeAll<AudioSource>())
        {
            if (audiosource.CompareTag("ConstantEffects"))
            {
                bearPlayer = audiosource;
            }

            if (audiosource.CompareTag("AudioEffects"))
            {
                bearPlayerEffects = audiosource;
            }
        }

        foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (canvas.CompareTag("HuntHUD"))
            {
                canvasHUD = canvas;
            }
        }        

        if (BearHUD != null)
        {
            BearHUD.SetActive(true);
        }
           

        if (PenguinHUD != null)
            PenguinHUD.SetActive(false);

        object property = false;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isPenguin", out property);

        if (!((bool)property))
        {
            staminaBarObject = canvasHUD.transform.Find("Stamina").gameObject as GameObject;
            staminaBarObject.gameObject.SetActive(true);
            staminaBar = staminaBarObject.transform.Find("StaminaFill").GetComponent<Image>(); //.transform.Find("StaminaFill").gameObject as Image;
            
            visionHud = canvasHUD.transform.Find("Berserker").gameObject as GameObject;
            visionHud.gameObject.SetActive(true); //cepo visible
            isBear = true;
        }

        //stockHud = canvasHUD.transform.Find("Stock").gameObject as GameObject;
        //stockHud.gameObject.SetActive(false); //cepo no visible


    }

    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }


        //ANIMACIÓN ANDAR
        // && (isRunning == false)
        if ((keysPressed > 0))
        {
            

            walking_animation = true;
        }
        else
        {
            //walking_animation = false;
            //bearPlayer.Stop();

        }
        
        if (bear_animator != null)
        {
            bear_animator.SetBool("walking", walking_animation);
        }
            
        //ANIMACIÓN ATACAR
        if (attacking == false)
        {
            attacking_animation = false;
        }
        else
        {
            attacking_animation = true;
        }

        if (bear_animator != null)
            bear_animator.SetBool("attacking", attacking_animation);

        //ANIMACIÓN CORRER
        if (isRunning == true)
        {
            running_animation = true;
        }
        else
        {
            running_animation = false;
        }

        if (bear_animator != null)
            bear_animator.SetBool("running", running_animation);

        // ANIMACIÓN DAÑO
        if (damaged == true)
        {
            damage_animation = true;
        }
        else
        {
            damage_animation = false;
        }

        if (bear_animator != null)
            bear_animator.SetBool("damaged", damage_animation);

        //if (_controls.Player.Run.canceled)
        //{
        if ((Keyboard.current.shiftKey.isPressed == false) && (keyboardRunning == true))// || (Gamepad.current.rightTrigger.isPressed == false) || (Gamepad.current.leftTrigger.isPressed == false))
        {
            isRunning = false;
            keyboardRunning = false;
        }


        staminaBar.fillAmount = stamina / maxStamina;   
            //bearPlayer
       
                //bearPlayer.Stop();
                
       
        //}
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

        if (_playerRB.transform.position.y > 133)
        {
            CameraDirection(); //movimiento respecto de la camara
            ThirdCamera(); //control cámara tercera persona
        }        

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
        if (Stamina != null)
            Stamina.fillAmount = stamina/600;
        //transform.LookAt(_playerRB.transform.position);
        //pivot.LookAt(target.position);
        //pivot.LookAt(target);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }    
        
        if (collision.gameObject.tag == "Corner1")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 20, _playerRB.position.z - 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Corner2")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 20, _playerRB.position.z - 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Corner3")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 20, _playerRB.position.z + 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Corner4")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 20, _playerRB.position.z + 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }


        if (collision.gameObject.tag == "Side1")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x, _playerRB.position.y + 25, _playerRB.position.z - 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Side2")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x, _playerRB.position.y + 25, _playerRB.position.z + 10));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Side3")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x + 10, _playerRB.position.y + 25, _playerRB.position.z));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if (collision.gameObject.tag == "Side4")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 10, _playerRB.position.y + 25, _playerRB.position.z));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }

        if(collision.gameObject.tag == "IceFall")
        {
            _playerRB.MovePosition(new Vector3(_playerRB.position.x - 5, _playerRB.position.y + 8, _playerRB.position.z - 5));
            bearPlayer.clip = fallInWater;
            bearPlayer.Play();

            if (isRunning == true)
            {
                isRunning = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (collision.gameObject.tag == "Fish") //Si choca con un cepo
        {
            stamina += 200;
            bearPlayerEffects.clip = eatFishEffect;
            bearPlayerEffects.Play();
            //Debug.Log("COLAS");
        }

        if (collision.gameObject.tag == "Penguin")  // Si colisiona con pinguino
        {
            if (attacking == true)   // Si está atacando
            {
                GameObject pengu = collision.gameObject;
                if (!PhotonNetwork.IsConnected) // Si es offline, lo elimina
                {
                    Destroy(pengu);
                    bearPlayerEffects.clip = penguinDeathEffect;
                    bearPlayerEffects.Play();
                } 
                else  // Si es online, le pide que se muera y le pasa información para comprobar que no hubo lag
                {
                    object[] objectArray = new object[2];
                    objectArray[0] = pengu.transform.position;
                    objectArray[1] = ownerActorNumber;
                    pengu.GetComponent<PhotonView>().RPC("ToDie", pengu.GetComponent<PhotonView>().Owner, objectArray as object);
                }
            }
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
            onFloor = false; //para que no suenen las pisadas

            bearPlayerEffects.clip = damageStockEffect;
            bearPlayerEffects.Play();
        }

        if(collision.gameObject.tag == "IceTrap")
        {
            onFloor = false;
            onIce = false;
            bearPlayerEffects.clip = crashIceEffect;
            bearPlayerEffects.Play();
        }

        if (collision.gameObject.tag == "IceDashPlat")
        {
            //bearPlayer.clip = iceDashEffect;
            //bearPlayer.Play();
            //bearPlayer.clip = iceDashEffect;
            //bearPlayer.Play();
            onFloor = false;
            onIce = true;
        }

        if (collision.gameObject.tag == "Floor")
        {

            //bearPlayer.clip = snowWalkEffect;
            //bearPlayer.Play();
            onIce = false;
            onFloor = true;
        }
    }

    #endregion

    #region HANDLER INPUTS
    //Asignamos las acciones mediante handlers
    private void OnEnable()
    {
        //Añade el evento moverse
        _controls.Player.ESC.performed += ESC;
        _controls.Player.Movement.performed += Move;
        _controls.Player.Attack.performed += Attack; //Evento atacar
        _controls.Player.Run.performed += Run; //Evento correr
        _controls.Player.PowerUp.performed += PowerUp; //Evento power up
        _controls.Player.CameraControl.performed += GetCameraMove;//Movimiento de camara

        //Habilita el evento
        if (!GetComponent<PhotonView>().IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        _controls.Player.Enable();
    }

    //Desactivamos la acción
    private void OnDisable()
    {
        //Elimina el evento
        _controls.Player.ESC.canceled -= ESC;
        _controls.Player.Movement.canceled -= Move;
        _controls.Player.Attack.canceled -= Attack;
        _controls.Player.Run.canceled -= Run;
        _controls.Player.PowerUp.canceled -= PowerUp;
        _controls.Player.CameraControl.canceled -= GetCameraMove;
        _controls.Player.Disable();

    }

    #endregion

    #region PLAYER ACTIONS

    public void ESC(InputAction.CallbackContext context)
    {
        Pause();
    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        attacking = true;
        _timeAtacking = Time.fixedTime + 1;
    }

    public void Run(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        if (firstTime == true)
        {
            stamina += Time.fixedTime;
            firstTime = false;
        }

        if (Keyboard.current.shiftKey.isPressed == true)
        {
            keyboardRunning = true;
        }
        if ((context.control.IsPressed() == true)) //(_controls.Player.Run != null)//((context.control.IsPressed(0) == true) || (context.control.IsPressed(1) == true)) 
        {
            Debug.Log("A correr");
            isRunning = true;
            speed = 10;
            _timeRunning = Time.fixedTime + 10;
            finalStamina = Time.fixedTime;
        }
        else
        {
            isRunning = false;
            Debug.Log("Dejo de correr");
        }
    }

    public void PowerUp(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //Si el power up esta aviable

        if (powerUpAviable == true)
        {
            if (isBear == true)
            {
                visionHud.gameObject.SetActive(false); //cepo visible
            }

            //ACTIVAR VISIÓN
            GameObject[] penguins = GameObject.FindGameObjectsWithTag("Penguin");
            Material[] visionMaterials = new Material[3];
            visionMaterials[0] = visionMaterial1;
            visionMaterials[1] = visionMaterial2;
            visionMaterials[2] = visionMaterial3;

            for (int i = 0; i < penguins.Length; i++)
            {
                Renderer rend = penguins[i].GetComponentInChildren<Renderer>();
                rend.materials = visionMaterials;
            }

            powerUpOn = true;
            powerUpAviable = false;
            _timePowerUp = Time.fixedTime + 5;
            cooldowmPowerUp = Time.fixedTime + 10;
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        //Debug.Log(context.control.device.displayName);
        _horizontaldirection = context.ReadValue<Vector2>();
        //bearPlayer.clip = snowWalkEffect;
        //bearPlayer.Play();

        if (context.control.IsPressed() == true)
        {
            keysPressed++;
            //onFloor = true;
        }
        else
        {
            keysPressed--;
        }
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
        moveY = Mathf.Clamp(moveY, -30.0f, 25.0f);

        //pivot sigue a player
        Vector3 follow = new Vector3(this.transform.position.x, this.transform.position.y /*- 1.0f*/, this.transform.position.z);
        pivot.position = Vector3.Lerp(pivot.position, follow /*+ offset*/, Time.deltaTime * 100);

        //Rotar pivot
        pivot.rotation = Quaternion.Euler(-moveY, moveX, 0.0f);
        //target.rotation = Quaternion.Euler(0,0, 0.0f);

    }

    #endregion

    #region BASIC ACTIONS MANAGEMENT

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


    //Hacer daño
    public void Stun()
    {
        //Si está corriendo, para
        if (isRunning == true)
        {
            isRunning = false;
        }

        //Consecuencias del daño
        _timeDamage = Time.fixedTime + 10;
        Debug.Log("DAÑO");
        speed = 0;
        _controls.Player.Run.Disable();
        _controls.Player.Movement.Disable();
        _controls.Player.CameraControl.Disable();
        damaged = true;
    }

    //Que el daño dure x tiempo
    public void ToDamage(double deltaTime)
    {
        if (damaged == true)
        {
            if (deltaTime > _timeDamage)
            {
                speed = 3;
                _controls.Player.Run.Enable();
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
            attacking = false;
        }
    }

    //Tiempo que dura la vision
    public void UsePoweUp (double deltaTime)
    {
        if (powerUpOn == true)
        {
            if (deltaTime > _timePowerUp)
            {
                GameObject[] penguins = GameObject.FindGameObjectsWithTag("Penguin");
                Material[] normalMaterials = new Material[3];
                normalMaterials[0] = normalMaterial1;
                normalMaterials[1] = normalMaterial2;
                normalMaterials[2] = normalMaterial3;

                for (int i = 0; i < penguins.Length; i++)
                {
                    Renderer rend = penguins[i].GetComponentInChildren<Renderer>();
                    rend.materials = normalMaterials;
                    //rend.material = normalMaterial;
                }

                powerUpOn = false;
            }
        }

        if (powerUpAviable == false)
        {
            if (deltaTime > cooldowmPowerUp)
            {
                powerUpAviable = true;

                if (isBear == true)
                {
                    visionHud.gameObject.SetActive(true); //cepo visible
                }
            }
        }
    }

    //Controla el tiempo que esta activo correr y la stamina
    public void ToRun(double deltaTime)
    {
        if (isRunning == true)
        {
            stamina--;

            if (finalStamina > stamina)
            {
                speed = 3;
                isRunning = false;
            }
        }
        else
        {
            if (damaged == false)
            {
                speed = 3;
            }

            if (stamina < deltaTime + 600)
            {
                stamina++;
            }

        }
    }

    #endregion

}


