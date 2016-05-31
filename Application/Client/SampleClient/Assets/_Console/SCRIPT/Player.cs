//-------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//-------------------------------------------------------------------------------------------------

public class Player
{
    public Vector2 Position { get; set; }
    public GameObject WorldObject;


    public Player()
    {
        Position = new Vector2(0, 0);
    }


    public void Move(Vector2 _direction)
    {
        Position += _direction;
        Position = new Vector2(RangeValue.Get(-50, 50, Position.x), RangeValue.Get(-50, 50, Position.y));

        WorldObject.transform.position = new Vector3(Position.x, Position.y);
    }
}
//-------------------------------------------------------------------------------------------------