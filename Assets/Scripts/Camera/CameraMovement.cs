using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private PlayerControls _controls;
    public Vector3 offset;
    public Vector2 movementCamera;
    public Transform target;
    public Transform pivot;
    private float moveX;
    private float moveY;
    private float m_LookSense = 1.0f;

    public Camera mainCamera;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void FixedUpdate()
    {
        ThirdCamera();

    }

    private void OnEnable()
    {
        //Movimiento de camara
        _controls.Player.CameraControl.performed += GetCameraMove;
        //Habilita el evento
        _controls.Player.Enable();
    }

    //
    private void OnDisable()
    {
        _controls.Player.CameraControl.canceled -= GetCameraMove;
        _controls.Player.Disable();

    }

    public void GetCameraMove(InputAction.CallbackContext context)
    {
        movementCamera = context.ReadValue<Vector2>();
    }

    public void ThirdCamera()
    {
        //Seguir al target
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position /*+ offset-*/, Time.deltaTime * 100);
        //Rotar la cámara
        mainCamera.transform.rotation = target.rotation;

        //Entrada del movimiento
        moveX += movementCamera.x * m_LookSense;
        moveY += movementCamera.y * m_LookSense;

        //limitar movimiento y entre -50 y 70
        moveY = Mathf.Clamp(moveY, -50.0f, 70.0f);

        //pivot sigue a player
        Vector3 follow = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        pivot.position = Vector3.Lerp(pivot.position, follow + offset, Time.deltaTime * 100);

        ///offset = Quaternion.AngleAxis(moveX * m_LookSense, Vector3.up) * offset;
        //Rotar pivot
        pivot.rotation = Quaternion.Euler(-moveY, moveX, 0.0f);


    }
}
