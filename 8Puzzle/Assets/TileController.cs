using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public List<GameObject> spaces;
    public GameObject emptySpace;
    public GameObject emptyTile;

    // Start is called before the first frame update
    void Start()
    {
        SetEmptySpace();
    }

    public RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject tile = hit.collider.gameObject;
                Debug.Log("Ray hit: " + tile.name);

                Vector3 adjacentDir = IsAdjacentToEmpty(tile);
                if (adjacentDir != Vector3.zero)
                {
                    MoveEmpty(adjacentDir);
                }
                else
                {
                    Debug.Log("Not adjacent to empty tile");
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveEmpty(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveEmpty(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveEmpty(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveEmpty(Vector3.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Shuffle();
        }

        
    }


    private Vector3[] cardinalDir = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    private float maxDirectionDist = 30;

    public Vector3 IsAdjacentToEmpty(GameObject tile)
    {
        RaycastHit adjacentHit;
        GameObject adjacentTile;
        foreach (Vector3 dir in cardinalDir)
        {
            if (Physics.Raycast(emptyTile.transform.position, dir, out adjacentHit, maxDirectionDist))
            {
                adjacentTile = adjacentHit.collider.gameObject;

                //Debug.DrawLine(emptyTile.transform.position + new Vector3(0,0,-5), dir);
                Debug.Log("Tile adjacent to Empty Tile: " + adjacentHit.collider.gameObject.name);
                if (adjacentTile == tile)
                {
                    return dir;
                }
            }
        }

        return Vector3.zero;
    }

    public int shuffleMin = 60;
    public int shuffleMax = 100;
    public void Shuffle()
    {
        for (int i = 0; i < Random.Range(shuffleMin, shuffleMax); i++)
        {
            MoveEmpty(cardinalDir[Random.Range(0,4)]);
        }
    }

    public void MoveEmpty(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(emptyTile.transform.position, direction, out hit, maxDirectionDist))
        {
            if (hit.collider.gameObject != null)
            {
                swapTilePositions(hit.collider.gameObject, emptyTile);
                Debug.Log("Move Empty Raycast hits: " + hit.collider.gameObject.name);
                SetEmptySpace();
            }
        }
    }

    public Vector3 swapPositionOffset = new Vector3(0, 0, -3);
    void swapTilePositions(GameObject tile1,  GameObject tile2)
    {
        Transform temp = tile1.transform.parent;
        tile1.transform.SetParent(tile2.transform.parent);
        tile1.transform.localPosition = swapPositionOffset;
        tile2.transform.SetParent(temp);
        tile2.transform.localPosition = swapPositionOffset;
    }

    void SetEmptySpace()
    {
        emptySpace = emptyTile.transform.parent.gameObject;
    }

    
}
