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
    public GameObject reference; //ref de la camara

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
         _playerRB.velocity = new Vector3(_horizontaldirection.x * speed, _playerRB.velocity.y, _horizontaldirection.y * speed);
       // _playerRB.AddForce(Vector3.right * speed * _horizontaldirection);

        //Para que no supere una velocidad:
        if(_playerRB.velocity.magnitude > maxSpeed)
        {
            _playerRB.velocity = _playerRB.velocity.normalized * maxSpeed;
        }
    }

    //Asignamos las acciones mediante handlers
    private void OnEnable()
    {
        //Añade el evento moverse
        _controls.Player.Movement.performed += Move;
        //Añade el evento atacar
        _controls.Player.Attack.performed += Attack;
        //Habilita el evento
        _controls.Player.Enable();
    }

    //
    private void OnDisable()
    {
        //Elimina el evento
        _controls.Player.Movement.performed -= Move;
        _controls.Player.Attack.performed -= Attack;
        _controls.Player.Disable();

    }

    public void Attack(InputAction.CallbackContext context) //De momento va a ser saltar
    {
        Debug.Log(context.control.device.displayName);
        _playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Move (InputAction.CallbackContext context) 
    {
        Debug.Log(context.control.device.displayName);
        _horizontaldirection = context.ReadValue<Vector2>();
    }



}
