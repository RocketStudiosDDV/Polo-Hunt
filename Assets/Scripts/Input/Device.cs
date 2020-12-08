using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Device : MonoBehaviour
{

    private Keyboard keyboard;
    private Mouse mouse;
    private Gamepad gamepad;

    // Start is called before the first frame update
    void Start()
    {
        //Obtener teclado, ratón y gamepad actuales
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        gamepad = Gamepad.current;      
    }

    // Update is called once per frame
    void Update()
    {
        //Para los botones, utilizar isPressed si queremos que sea mientras está presionada y wasPressedThisFrame si solo tiene que activarse una vez al ser presionada dentro del update del frame actual (ejemplo una cosa q eueremos que pase una vez y fin)
        //El wasPressedThisFrame puede servir para el deslizamiento, contar ene l momento en que se le da click y cuando haya pasado x tiempo, se tiene que levantar
        //Para hacer lo que sea cuando suelto la tecla: wasRealasedThisFrame
        //mouse.position.readValue() -> te dice la pos del raton (esto pa la camara)

        //Comprobamos entradas de teclado
        if (keyboard.wKey.isPressed)
        {
            Debug.Log("W pressed");
        }

        if (keyboard.aKey.isPressed)
        {
            Debug.Log("A pressed");
        }

        if (keyboard.sKey.isPressed)
        {
            Debug.Log("S pressed");
        }

        if (keyboard.dKey.isPressed)
        {
            Debug.Log("D pressed");
        }

    }
}
