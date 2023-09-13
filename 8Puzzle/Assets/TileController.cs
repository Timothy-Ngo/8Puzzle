using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TileController : MonoBehaviour
{
    public const int MAXROW = 3;
    public const int MAXCOL = 3;

    public GameState currentState;
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

    public GameObject[,] spaceMatrix = new GameObject[MAXROW, MAXCOL]; // Store's the tiles in each space
    //Don't use space matrix

    // Start is called before the first frame update
    void Start()
    {
        currentState = new GameState();
        
        foreach(GameObject space in spaceObjects)
        {
            tiles.Add(space.GetComponentInChildren<Tile>());
        }

        UpdateBoardData();
        
        SetEmptySpace();
    }
    public void PrintTiles() 
    {
        foreach(Tile tile in tiles)
        {
            Debug.Log(tile.GetNum());
        }
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
        int index = 0;
        foreach (Tile tile in tiles)
        {
            currentState.gameBoard[tile.position] = tile.GetNum();
        }
        //currentState.PrintGameBoardData();
    }
    
    /*
    void InitBoardData()
    {
        int index = 0;
        for (int row = 0; row < MAXROW; row++)
        {
            for (int col = 0; col < MAXCOL; col++)
            {
                if (index == 8)
                {
                    break;
                }
                //Debug.Log(index);
                string name = (index + 1).ToString();
                Debug.Log(name);
                spaceMatrix[row, col] = spaces[index].transform.Find(name).gameObject;
                Debug.Log(spaceMatrix[row, col]);
                index++;
            }
        }

        spaceMatrix[MAXROW - 1, MAXCOL - 1] = spaces[index].transform.Find("0").gameObject;
    }
    */

    int EmptyMatrixPos()
    {
        for (int row = 0; row < MAXROW; row++)
        {
            for (int col = 0; col < MAXCOL; col++)
            {
                if (spaceMatrix[row, col] == emptyTile.transform.parent.gameObject)
                {
                    //Debug.Log(spaceMatrix[row, col]);
                    string num = row.ToString() + col.ToString();
                    return int.Parse(num);
                }
            }
        }

        return -1; // Empty could not be found

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
        UpdateBoardData();
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
                SetEmptySpace();
                UpdatePositions();
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

    public void SetEmptySpace()
    {
        //emptySpace = emptyTile.transform.parent.gameObject;
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
    int emptyMatrixPosition;
    public GameState()
    {
        InitBoardData();
    }

    public GameState(Dictionary<int, int> newBoard)
    {
        gameBoard = newBoard;

        emptyMatrixPosition = GetEmptyMatrixPosition();
    }
    public Dictionary<int, int> ApplyMove(Vector3 move) // Not Tested
    {
        Dictionary<int, int> oldBoard = gameBoard;
        Dictionary<int, int> newBoard = new Dictionary<int, int>();
        int emptyPos = GetEmptyMatrixPosition();

        if (move == Vector3.up)
        {
            SwapTilePositions(emptyPos, emptyPos - 10);
        }
        else if (move == Vector3.down)
        {
            SwapTilePositions(emptyPos, emptyPos + 10);
        }
        else if (move == Vector3.left)
        {
            SwapTilePositions(emptyPos, emptyPos - 1);
        }
        else if (move == Vector3.right)
        {
            SwapTilePositions(emptyPos, emptyPos + 1);
        }
        newBoard = gameBoard;
        gameBoard = oldBoard;

        return newBoard;
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

    public void SwapTilePositions(int pos1, int pos2)
    {
        int temp = gameBoard[pos1];
        gameBoard[pos1] = gameBoard[pos2];
        gameBoard[pos2] = temp;
    }
    public int GetEmptyMatrixPosition()
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


    public List<Vector3> PossibleMoves() // Not Tested
    {
        int emptyPos = GetEmptyMatrixPosition();
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

    /*
    public override Dictionary<int,int> GetHashCode()
    {
        return gameBoard;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    */
}