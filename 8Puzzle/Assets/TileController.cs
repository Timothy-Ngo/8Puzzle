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

    [Header("Shuffle Settings")]
    //public int shuffleMax = 20;
    public bool doShuffle = false;
    public float timePerMove = 1.0f;
    public float shuffleTimer;
    private Vector3 prevVector = Vector3.zero;
    [Header("Misc")]
    [Tooltip("Local offset position for tile object to be rendered from the Space parent object.")]
    public Vector3 swapPositionOffset = new Vector3(0, 0, -3);
    [Tooltip("Utilized for mouse click functionality to obtain information about what tile object is hit.")]
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




        // PrintTiles
        //currentState = new GameState();
        //UpdateEverything();
        //currentState.PrintGameBoardData();

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
            doShuffle = !doShuffle;
            shuffleTimer = timePerMove;
        }

        if (doShuffle)
        {
            shuffleTimer -= Time.deltaTime;
            if (shuffleTimer <= 0)
            {
                prevVector = Shuffle(prevVector);
                shuffleTimer = timePerMove;
            }
        }

        
    }
    //------------------------------------------------------------------------------------------------------------
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
    public Vector3 Shuffle(Vector3 prevVector)
    {
        /*
        for (int i = 0; i < Random.Range(shuffleMin, shuffleMax); i++)
        {
            MoveEmpty(cardinalDir[Random.Range(0,4)]);
        }
        */
        Vector3 randomVector = cardinalDir[Random.Range(0, 4)];

        while (randomVector == (-prevVector))
        {
            randomVector = cardinalDir[Random.Range(0, 4)];
        }

        MoveEmpty(randomVector);

        return randomVector;
        
    }
    //------------------------------------------------------------------------------------------------------------
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
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------

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

    
}
/*
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
        for (int i = 0; i < 9; i += 3)
        {
            Debug.Log(gameBoard[matrixPositions[i]] + " " + gameBoard[matrixPositions[i+1]]+  " " + gameBoard[matrixPositions[i + 2]] );
        }
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
    */