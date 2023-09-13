using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int position; // Parent Game Object name in the form of a matrix position
    // Start is called before the first frame update
    void Start()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        position = int.Parse(transform.parent.gameObject.name);
        //Debug.Log("Tile " + name + " is in position " + position);
    }

    public int GetNum()
    {
        return int.Parse(name);
    }
}
