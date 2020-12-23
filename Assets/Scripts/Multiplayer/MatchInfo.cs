using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour
{
    #region VARIABLES

    public int penguinsAlive; //pinguinos restantes
    public int bearsConnected; //osos conectados
    public int penguinsConnected; //pinguinos conectados

    private double _totalTime = 0; //tiempo que dura la partida

    #endregion

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        endTime(Time.fixedTime);
        endGame();
        
    }

    #region METHODS

    //RPC que comprobara cuantps pingguinos hay vivvos y los actualizara
    //cuando el master sepa que han maatdo a un pingu, hace que todos llamen a esto
    public void ActualiceNumPenguins (int penguins)
    {
        //MANU
    }

    //modo espectador -> aunque haya game over que puedas seguir la camañar de la persona que te ha matado
    //le sigues y encima se ve un game over y a la dcha boton de espectar o salir
    //si le das a espectar se quita el game over y si le das a salir vas a main menu
    public void SpectatorMode(GameObject pinguino, GameObject oso)
    {
        //PAULA
        //intsnacia de la pinga muerta asignarle a camara la del oso y apaghar todos los controles
        //find el object of type y asignarla al oso
        //pinguino funcion muerte
        //pinguino al morir invoca a esta funcion

       /* Camera newCamera = pinguino.GetComponent<Camera>();
        newCamera = oso.GetComponent<Camera>();
        */
    }


    //método que se llamará cuando el juego decida que se acabara la partida y se muestra a quien siga dntro de la partida
    public void ShowResults()
    {
        //Paula / tomas
    }

    //inicia la partida
    public void StartGame()
    {
        _totalTime = _totalTime + 600; //le sumamos en segs el tiempo que dura la partida
        //inicializar posiciones de los pinguinos y cosas del mapa (pescados y así)

        //MANU
    }

    //Que ubique los requisitos de fin de partida
        //tiempo restante -> se acaba
        //Solo qudan pinguinos
    //El master avisa a todos de endMatch en caso de que el tiempo se haya acabado
    public void endGame()
    {

        //MANU 

        if (penguinsAlive == 0)
        {
            //se acaba Y SE LLAMARA A LA PUNTUACION - SHOW REUSLTS
        }

        if (penguinsConnected == 0)
        {
            //se acaba
        }

        if (bearsConnected == 0)
        {
            //se acaba
        }
    }
    //cuando se acaba el tiempo
    public void endTime(double deltaTime)
    {

        //MANU

        if (deltaTime > _totalTime)
        {
            //se acaba la partoida
        }
    }
    #endregion
}
