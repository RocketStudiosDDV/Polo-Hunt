using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogWriter : MonoBehaviour
{
    /// <summary>
    /// Debe estar marcado para escribir
    /// </summary>
    public bool verboseMode;
    /// <summary>
    /// Escribe en consola en vez de en el log
    /// </summary>
    public bool writeOnConsole;
    public Text log;

    public void Start()
    {
        if (verboseMode == true && log == null)
        {
            Debug.LogWarning("LogWriter debe tener la referencia a un objeto text para imprimir datos.\n" +
                "Marcar verboseMode a false en el inspector para no imprimir por pantalla");
        }
    }

    /// <summary>
    /// Escribe el mensaje en el log
    /// time = true para que muestre la hora
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void Write(System.Object text, bool time = false)
    {
        if (verboseMode)
        {
            if (writeOnConsole)
            {
                if (time == true)
                {
                    Debug.Log(Time.time + text.ToString());
                } else
                {
                    Debug.Log(text.ToString());
                }
            } else
            {
                log.text += "\n";
                if (time == true)
                {
                    log.text += "[ " + Time.time + " ]";
                }
                log.text += text.ToString();
            }
            
        }
    }

    /// <summary>
    /// Resetea el contenido del log
    /// </summary>
    public void ClearLog()
    {
        if (!writeOnConsole)
        {
            log.text = "";
        }
    }
}
