//-------------------------------------------------------------------
// Name: Timothy Ngo
// School Email: timothyngo@nevada.unr.edu
//-------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileController : MonoBehaviour
{
    

    [Header("Gameboard Data")]
    public List<GameObject> spaceObjects;
    public Tile emptyTile;
    [Tooltip("Contains all numbered tiles")]
    public List<Tile> tiles;
    public List<Position> gameBoard;

    
    [Header("Misc")]
    [Tooltip("Local offset position for tile object to be rendered from the Space parent object.")]
    public Vector3 swapPositionOffset = new Vector3(0, 0, -3);
    public AudioSource tileMoveSfx;
    

    public RaycastHit hit;
    private Vector3[] cardinalDir = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    private float maxDirectionDist = 30;

    //-----------DEBUGGING----------------------------------------------------------------------------------------
    
    public void PrintTiles()
    {
        Debug.Log("-----------Print Tiles-----------------");
        foreach (Tile tile in tiles)
        {
            tile.Print();
        }
        PrintEmptyTile();
    }    

    public void PrintEmptyTile()
    {
        Debug.Log("Empty Tile, " + emptyTile.position);
    }

    public void PrintBoard(List<Position> board)
    {
        Debug.Log("-----------Print Board-----------------");
        for (int i = 0; i < board.Count; i++)
        {
            Debug.Log("Tile " + i + " is at position " + board[i]); 
        }
    }

    //-------DATA CODE--------------------------------------------------------------------------------------------

    
    public void MoveEmpty(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(emptyTile.transform.position, direction, out hit, maxDirectionDist))
        {
            if (hit.collider.gameObject != null)
            {
                SwapTilePositions(hit.collider.gameObject, emptyTile.gameObject);
                //Debug.Log("Move Empty Raycast hits: " + hit.collider.gameObject.name);
                //UpdateEverything();
                tileMoveSfx.Play();
            }
        }
    }
    void SwapTilePositions(GameObject tile1,  GameObject tile2)
    {
        // Changes Object
        Transform temp = tile1.transform.parent;
        tile1.transform.SetParent(tile2.transform.parent);
        tile1.transform.localPosition = swapPositionOffset;
        tile2.transform.SetParent(temp);
        tile2.transform.localPosition = swapPositionOffset;

        // Changes Tile 
        Position tempPos = tile1.GetComponent<Tile>().position;
        tile1.GetComponent<Tile>().position = tile2.GetComponent<Tile>().position;
        tile2.GetComponent<Tile>().position = tempPos;

        // Updates Board
        SyncBoardWithTiles();

        // Debugging
        //PrintTiles();
        //PrintEmptyTile();
        //PrintBoard();
    }
    public void SyncBoardWithTiles() // Dependent on the Tiles numbers staying readonly
    {
        gameBoard[0] = emptyTile.position;
        
        for (int i = 1; i < 9; i++)
        {
            gameBoard[i] = tiles[i-1].position;
        }
        
    }

    //-------GAME CODE----------------------------------------------------------------------------------------
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                GameObject tile = hit.collider.gameObject;
                //Debug.Log("Ray hit: " + tile.name);

                Vector3 adjacentDir = IsAdjacentToEmpty(tile);
                if (adjacentDir != Vector3.zero)
                {
                    MoveEmpty(adjacentDir);
                }
                else
                {
                    //Debug.Log("Not adjacent to empty tile");
                }

            }

        }

        /*
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
        */

        

        
    }
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
                //Debug.Log("Tile adjacent to Empty Tile: " + adjacentHit.collider.gameObject.name);
                if (adjacentTile == tile)
                {
                    return dir;
                }
            }
        }

        return Vector3.zero;
    }
    //------------------------------------------------------------------------------------------------------------
   


    
}