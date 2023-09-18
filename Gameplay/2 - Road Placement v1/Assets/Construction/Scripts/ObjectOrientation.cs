using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
    Gives orientation to objects (NSEW) and can make the gameobject face any orientation.
    May only work properly in dynamically updating objects of 1x1 space like roads.
*/

public class ObjectOrientation : MonoBehaviour
{

    public enum Cardinal_Direction
    {
        North,
        South,
        East,
        West
    }

    /// With a cardinal origin and a cardinal destination, we can get the degrees we need to rotate an object to face
    /// that way.
    private Dictionary<Tuple<Cardinal_Direction, Cardinal_Direction>, int> rotation_rules = new Dictionary<Tuple<Cardinal_Direction, Cardinal_Direction>, int>
    {
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.North, Cardinal_Direction.South), 180},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.North, Cardinal_Direction.East), 90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.North, Cardinal_Direction.West), -90},

        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.South, Cardinal_Direction.North), 180},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.South, Cardinal_Direction.East), -90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.South, Cardinal_Direction.West), 90},

        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.East, Cardinal_Direction.North), -90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.East, Cardinal_Direction.South), 90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.East, Cardinal_Direction.West), 180},

        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.West, Cardinal_Direction.North), 90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.West, Cardinal_Direction.South), -90},
        {new Tuple<Cardinal_Direction, Cardinal_Direction>(Cardinal_Direction.West, Cardinal_Direction.East), 180}
    };

    [SerializeField] private Cardinal_Direction current_cardinal_direction = Cardinal_Direction.North;

    public void change_cardinal_direction(Cardinal_Direction new_cardinal_direction)
    {
        if (current_cardinal_direction == new_cardinal_direction)
        {
            return;
        }

        int rotation_degrees = rotation_rules[Tuple.Create(current_cardinal_direction, new_cardinal_direction)];

        transform.Rotate(new Vector3(0, rotation_degrees, 0));

        current_cardinal_direction = new_cardinal_direction;
    }

    public void rotate_clockwise()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }

    public void rotate_counterclockwise()
    {
        transform.Rotate(new Vector3(0, -90, 0));
    }

}
