using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public enum Position
{
    TopLeft = 00,
    TopMiddle = 01,
    TopRight = 02,
    MiddleLeft = 10,
    Center = 11,
    MiddleRight = 12,
    BottomLeft = 20,
    BottomMiddle = 21,
    BottomRight = 22
}

public class Tile : MonoBehaviour
{
    public int number; 
    public Position startPosition;
    public Position position;
    void Start()
    {
        position = startPosition;
        
    }

    public void SetPosition(Position pos)
    {
        position = pos;
    }

    // DEBUGGING

    public void Print()
    {
        Debug.Log("Tile " + number + ", Position " + position);
    }

}

