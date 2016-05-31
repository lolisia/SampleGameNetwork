//-------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//-------------------------------------------------------------------------------------------------

public class Controller : MonoBehaviour
{
    public Player ControllablePlayer;


    public void Move(Vector2 _direction)
    {
        ControllablePlayer.Move(_direction);
    }

    void Update()
    {
        Move(new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)));
    }
}
//-------------------------------------------------------------------------------------------------