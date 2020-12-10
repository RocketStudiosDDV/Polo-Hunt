using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceActions : MonoBehaviour
{

    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 3;
    [SerializeField] float maxSpeed = 10;

    private PlayerControls _controls;
    private Rigidbody _playerRB; //Rigid body del pingu

    private Vector2 _horizontaldirection;
    private Vector3 playerInput; //guarda la info del input
    private Vector3 playerDirection; //Hacia donde se mueve el jugador

    //public GameObject reference; //ref de la camara

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

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
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

        CameraDirection();
        ThirdCamera();
        //transform.LookAt(target);

        target.transform.LookAt(pivot);

        //Aplicamos la velocidad de movimiento WASD
        _playerRB.velocity = new Vector3(playerDirection.x * speed, _playerRB.velocity.y, playerDirection.z * speed);
        //_playerRB.AddForce(Vector3.right * speed * _horizontaldirection);

        

        Debug.Log(_playerRB.velocity.magnitude);
    }

    private void LateUpdate()
    {
        //transform.LookAt(_playerRB.transform.position);
        //pivot.LookAt(target.position);
        //pivot.LookAt(target);
    }

    //Asignamos las acciones mediante handlers
    private void OnEnable()
    {
        //Añade el evento moverse
        _controls.Player.Movement.performed += Move;
        //Añade el evento atacar
        _controls.Player.Attack.performed += Attack;
        //Movimiento de camara
        _controls.Player.CameraControl.performed += GetCameraMove;
        //Habilita el evento
        _controls.Player.Enable();
    }

    //
    private void OnDisable()
    {
        //Elimina el evento
        _controls.Player.Movement.canceled -= Move;
        _controls.Player.Attack.canceled -= Attack;
        _controls.Player.CameraControl.canceled -= GetCameraMove;
        _controls.Player.Disable();

    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        //Debug.Log(context.control.device.displayName);
        _playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Move (InputAction.CallbackContext context) 
    {
       //Debug.Log(context.control.device.displayName);
        _horizontaldirection = context.ReadValue<Vector2>();
    }

    public void GetCameraMove(InputAction.CallbackContext context) 
    {
        movementCamera = context.ReadValue<Vector2>();
    }

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

    



}
