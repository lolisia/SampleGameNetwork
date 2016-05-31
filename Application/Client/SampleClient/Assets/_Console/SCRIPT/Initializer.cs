//-------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//-------------------------------------------------------------------------------------------------

public class Initializer : MonoBehaviour
{
    public Controller Controller;
    public GameObject PlayerObject;


    void Start()
    {
        DevConsole.Console.ExecuteCommand("DC_SHOW_TIMESTAMP true");
        //DevConsole.Console.Open();

        Controller.ControllablePlayer = new Player();
        Controller.ControllablePlayer.WorldObject = PlayerObject;
    }
}
//-------------------------------------------------------------------------------------------------