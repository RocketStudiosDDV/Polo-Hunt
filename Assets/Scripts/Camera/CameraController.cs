using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerControls _controls;
    public GameObject player;
    public GameObject reference; //la camara maneja a la referencia para tener los ejes bien colocados
    public Vector3 distance; //distancia que mantioene la camar con el jugador


    // Start is called before the first frame update
    void Start()
    {
        distance = transform.position - player.transform.position; //Devuelve el vector que va del jugador a la camara
    }

    //Se asegura de que el jugador se movio
    private void LateUpdate()
    {
        
        transform.position = player.transform.position + distance;
        transform.LookAt(player.transform.position);

        //Esta es la referencia paera que los controles no cambien, que esten bien colocardos
        Vector3 copyRotation = new Vector3(0, transform.eulerAngles.y, 0);
        reference.transform.eulerAngles = copyRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        _controls.Player.Camera.performed += MoveCamera;
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Camera.performed -= MoveCamera;
        _controls.Player.Disable();
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        Vector2 num = context.ReadValue<Vector2>();
        //Calcula lo que se tiene que mover la camara con respecto al personaje y la mueve
        distance = Quaternion.AngleAxis(num.x * 2, Vector3.up) * distance;
        
    }
}
