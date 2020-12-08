using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceActions : MonoBehaviour
{

    /*[SerializeField] private InputAction movement; //Movimiento simple
    [SerializeField] private InputAction moveCamera; //Movimiento de la camara
    [SerializeField] private InputAction attack; //Ataque (bofetada/puño)
    [SerializeField] private InputAction run; //Correr/deslizarse
    [SerializeField] private InputAction powerUp; //Utilizar el power UP*/

    [SerializeField] float jumpForce = 10;
    [SerializeField] float speed = 2;

    private PlayerControls _controls;
    private Rigidbody _playerRB; //Rigid body del pingu
    private Vector2 _horizontaldirection;

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

  

    #region VIDEO 1
    /* //Habilita todos los handlers y eso
     private void OnEnable()
     {
         movement.type.
         //Añadimos el handler
         movement.performed += Move;
         //Lo habilitamos
         movement.Enable();

         //Añadimos el handler de atacar en función si se usa o no
         attack.performed += Attack;
         attack.canceled += DisableAttack;
         //Lo habilitamos
         attack.Enable();
     }

     //Desabilita todos los handlers y eso
     private void OnDisable()
     {
         //Elminamos el handler
         movement.performed -= Move;
         //Lo deshabilitamos
         movement.Disable();

         //Elminamos el handler de atacar en función si se usa o no
         attack.performed -= Attack;
         attack.canceled -= DisableAttack;
         //Lo habilitamos
         attack.Disable();
     }

     //Handler del movmiento
     private void Move(InputAction.CallbackContext context)
     {
         //Objetener valor de los ejes
         Vector2 direction = context.ReadValue<Vector2>();
         Debug.Log(direction);
         Vector3 vectorDirection = new Vector3(direction.x, 0, direction.y);

         transform.Translate(vectorDirection * Time.deltaTime);
     }

     //Handlers ataque
     private void Attack(InputAction.CallbackContext context)
     {
         transform.Translate(Vector3.up);
     }

     private void DisableAttack(InputAction.CallbackContext context)
     {
         transform.Translate(Vector3.down);
     }*/
    #endregion

}
