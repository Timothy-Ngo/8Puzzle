using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TileController : MonoBehaviour
{
    public const int MAXROW = 3;
    public const int MAXCOL = 3;




    //public GameState currenttate;
    public Dictionary<int, int> solvedBoard = new Dictionary<int, int>
    {
        {0, 1},
        {1, 2},
        {2, 3},
        {10, 4},
        {11, 5},
        {12, 6},
        {20, 7},
        {21, 8},
        {22, 0}

    };
    
    public List<GameObject> spaceObjects;
    public List<Tile> tiles;
    public Tile emptyTile;
    public List<int> currentState = new List<int>();
    
    // Start is called before the first frame update
    void Start()
    {
        //currentState = new GameState();

        UpdateEverything();
        //currentState.PrintGameBoardData();

    }
    public void PrintTiles() 
    {
        foreach(Tile tile in tiles)
        {
            Debug.Log(tile.GetNum());
        }
    }
    
    public void UpdateEverything()
    {
        UpdatePositions();
        UpdateBoardData();
    }

    void UpdatePositions()
    {
        foreach (Tile tile in tiles)
        {
            tile.UpdatePosition();
        }
    
    }

    
    void UpdateBoardData()
    {
        foreach(Tile tile in tiles)
        {
            currentState.Add(tile.position);
        }

        for (int tileNum = 0; tileNum < currentState.Count ; tileNum++)
        {
            currentState[tileNum] = tiles[tileNum].position;
        }
        currentState[0] = emptyTile.position;
        
        //Debug.Log(currentState.gameBoard.Count);
        //currentState.PrintGameBoardData();
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
                //Debug.Log("Tile adjacent to Empty Tile: " + adjacentHit.collider.gameObject.name);
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
        UpdateEverything();
    }

    public void MoveEmpty(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(emptyTile.transform.position, direction, out hit, maxDirectionDist))
        {
            if (hit.collider.gameObject != null)
            {
                swapTilePositions(hit.collider.gameObject, emptyTile.gameObject);
                //Debug.Log("Move Empty Raycast hits: " + hit.collider.gameObject.name);
                UpdateEverything();
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

    
}

public class GameState
{
    public Dictionary<int, int> gameBoard = new Dictionary<int, int>(); // <Space Matrix Position, Tile Number>
    public List<int> matrixPositions = new List<int> {
        0, 1, 2,
        10, 11, 12,
        20, 21, 22
    };
    public int emptyMatrixPosition;
    public GameState()
    {
        InitBoardData();
    }
    public GameState(GameState newState)
    {
        gameBoard = new Dictionary<int, int>(newState.gameBoard);
        emptyMatrixPosition = newState.emptyMatrixPosition;
    }
    public GameState(Dictionary<int, int> newBoard)
    {
        gameBoard = new Dictionary<int,int>(newBoard);

        emptyMatrixPosition = GetEmptyMatrixPosition();
    }
    
    public bool IsGoalState() // Not Tested
    {
        if (gameBoard[22] != 0)
        {
            return false;
        }
        int tileNum = 1;
        foreach (int position in matrixPositions)
        {
            if (position == 22)
            {
                break;
            }
            if (gameBoard[position] != tileNum)
            {
                return false;
            }
            tileNum++;
        }
        return true;
    }

    public void PrintGameBoardData()
    {
        Debug.Log("---------------------------------------------");
        for (int i = 1; i < 6; i += 3)
        {
            Debug.Log(gameBoard[matrixPositions[i]] + " " + gameBoard[matrixPositions[i+1]]+  " " + gameBoard[matrixPositions[i + 2]] );
        }
        Debug.Log(gameBoard[matrixPositions[7]] + " " + gameBoard[matrixPositions[8]] + " " + gameBoard[matrixPositions[0]]);
        Debug.Log("---------------------------------------------");
    }

    public int GetEmptyMatrixPosition() // Can be optimized
    {
        foreach (int position in matrixPositions)
        {
            if (gameBoard[position] == 0)
            {
                return position;
            }
        }
        Debug.Log("Could not find Empty Matrix Position");
        return -1; // Could not find the empty Position
    }

    public void InitBoardData() // Not Tested
    {
        int tile = 1;
        foreach (int position in matrixPositions)
        {
            if (position == 22)
            {
                gameBoard[22] = 0; // 0 represents an empty tile
                emptyMatrixPosition = position;
                break;
            }
            gameBoard[position] = tile++;
        }
        //Debug.Log(gameBoard);
    }


    public List<Vector3> PossibleMoves() // Not 
    {
        int emptyPos = emptyMatrixPosition;
        List<Vector3> possibleMoves = new List<Vector3>();
        if (matrixPositions.Contains(emptyPos - 10))
        {
            possibleMoves.Add(Vector3.up);
        }
        if (matrixPositions.Contains(emptyPos + 10))
        {
            possibleMoves.Add(Vector3.down);
        }
        if (matrixPositions.Contains(emptyPos - 1))
        {
            possibleMoves.Add(Vector3.left);
        }
        if (matrixPositions.Contains(emptyPos + 1))
        {
            possibleMoves.Add(Vector3.right);
        }
        return possibleMoves;
    }

}