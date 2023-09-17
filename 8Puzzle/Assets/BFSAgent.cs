using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditorInternal;
using UnityEngine;

enum Move
{
    up, down, left, right
}

public class BFSAgent : MonoBehaviour
{
    public TileController tc;

    private List<Position> solvedState = new List<Position>()
    {
        Position.BottomRight,
        Position.TopLeft,
        Position.TopMiddle,
        Position.TopRight,
        Position.MiddleLeft,
        Position.Center,
        Position.MiddleRight,
        Position.BottomLeft,
        Position.BottomMiddle
    };

    [Header("BFS Solver")]
    public float maxMoveTime = 0.5f;
    public float currentMoveTime;
    Stack<Vector3> solutionMoves = new Stack<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        currentMoveTime = maxMoveTime;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Key Pressed");
            //Debug.Log(ListToString(tc.gameBoard));
            BFS(tc.gameBoard);
            ReconstructMoves(parentDict);
            Debug.Log("Top of Stack" + solutionMoves.Peek());
            /*
            // Debug to see if NewState() works correctly
            List<Position> newState = new List<Position>();
            foreach (Position position in PossibleMoves(tc.gameBoard))
            {
                newState = NewState(tc.gameBoard, position);
                //PrintBoard(newState);
            }
            */
            /*
            // Debug to see if PossibleMoves() works correctly
            List<Position> positions = new List<Position>();
            positions = PossibleMoves(tc.gameBoard);
            foreach (Position position in positions)
            {
                Debug.Log(position);
            }
            */
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            BFSMove();
        }
    }
    //---------------------DEBUGGING-------------------------------------------------
    public void PrintStringState(string state)
    {
        Debug.Log(state);
    }
    //-------------------------------------------------------------------------------

    
    public void BFSMove()
    {
        Debug.Log(solutionMoves.Peek());
        tc.MoveEmpty(solutionMoves.Pop());
    }

    //-------------------------------------------------------------------------------
    public void ReconstructMoves(Dictionary<string, int> parents)
    {
        Stack<Vector3> solutionPathOfMoves = new Stack<Vector3>();
        string node = ListToString(solvedState);
        string initialStringState = ListToString(tc.gameBoard);
        while (node != initialStringState)
        {
            solutionMoves.Push(FindMove(parents[node],(int)EmptyPositionFromString(node)));
            Debug.Log(FindMove(parents[node], (int)EmptyPositionFromString(node)));
            node = ListToString(NewState(StringToList(node), (Position)parents[node]));
        }

    }

    public Vector3 FindMove(int parent, int child)
    {
        int diff = parent - child;
        Vector3 resultVector = Vector3.zero;
        if (diff == 1) 
        {
            resultVector = Vector3.left;
        }
        else if (diff == -10)
        {
            resultVector = Vector3.down;
        }
        else if (diff == -1)
        {
            resultVector = Vector3.right;
        }
        else if (diff == 10)
        {
            resultVector = Vector3.up;
        }
        return resultVector; 
    }

    public Position EmptyPositionFromString(string state)
    {
        state = state.Trim();
        string currentElement = string.Empty;
        Position emptyPosition = Position.Center; // Will return center if the loop doesn't work properly
        foreach(char c in state)
        {
            if (c == '.')
            {
                emptyPosition = (Position)int.Parse(currentElement);
                break;
            }
            else 
            {
                currentElement += c;
            }
        }

        return emptyPosition;
    }
    public List<Position> StringToList(string state)
    {
        state = state.Trim();
        string currentElement = string.Empty;
        List<Position> resultList = new List<Position>();
        foreach (char c in state)
        {
            if (c == '.')
            {
                resultList.Add((Position)int.Parse(currentElement));
                currentElement = string.Empty;
                
            }
            else
            {
                currentElement += c;
            }

        }
        return resultList;

    }

    //-------------------------------------------------------------------------------
    Dictionary<string, int> parentDict = new Dictionary<string, int>();

    public bool BFS(List<Position> initialState)
    {
        Queue<List<Position>> frontier = new Queue<List<Position>>();
        HashSet<string> stringFrontier = new HashSet<string>(); 
        HashSet<string> explored = new HashSet<string>();
        Dictionary<string, int> parents = new Dictionary<string, int>();
        if (IsGoalState(initialState))
        {
            Debug.Log("GOAL STATE FOUND");
            return true;
        }
        frontier.Enqueue(initialState);
        stringFrontier.Add(ListToString(initialState));

        while (frontier.Count > 0)
        {
            if (frontier.Count == 0)
            {
                Debug.Log("A solution was not found");
                return false;
            }
            List<Position> currentState = frontier.Dequeue();
            string currentStateString = ListToString(currentState);
            stringFrontier.Remove(currentStateString);
            explored.Add(currentStateString);

            //Debug.Log(visited.Count);
            foreach (Position position in PossibleMoves(currentState))
            {
                List<Position> newState = NewState(currentState, position);
                string newStringState = ListToString(newState);
                if (!(explored.Contains(newStringState) || stringFrontier.Contains(newStringState)))
                {
                    parents[newStringState] = (int)currentState[0];
                    if (IsGoalState(newState))
                    {
                        Debug.Log("GOAL STATE FOUND");
                        parentDict = new Dictionary<string, int>(parents);
                        return true; 
                    }
                    frontier.Enqueue(newState);
                    //Debug.Log("Added");
                    //Debug.Log(frontier.Count);
                }
            }
            if (explored.Count >= 150000)
            {
                Debug.Log("# of Explored States: " + explored.Count);
                Debug.Log("# of States in Frontier: " + frontier.Count);
                break;  
            }

        }
        Debug.Log("Could not find solution");
        return false;
    }


    public string ListToString(List<Position> board)
    {
        string resultString = string.Empty;
        foreach(Position position in board)
        {
            resultString += ((int)position).ToString() + ".";
        }
        return resultString;
        
    }
    //-------------------------------------------------------------------------------

    public bool IsGoalState(List<Position> board)
    {
        if (board[0] != Position.BottomRight)
        {
            return false;
        }

        int i = 1;
        foreach(Position pos in Enum.GetValues(typeof(Position)))
        {
            if (pos == Position.BottomRight)
            {
                break;
            }
            if (board[i] != pos)
            {
                return false;
            }
            i++;
        }

        return true;
    }
    //-------------------------------------------------------------------------------

    public List<Position> PossibleMoves(List<Position> board)
    {
        // Find Empty tile position
        // Use position math to determine which position the empty tile can move
        // Add move to list<move> if the emptytile can move that way
        List<Position> possibleMoves = new List<Position>();
        Position emptyTilePos = board[0];
        
        if (Enum.IsDefined(typeof(Position), emptyTilePos - 1)) // Can the Empty Tile go left?
        {
            possibleMoves.Add((Position) emptyTilePos - 1);
        }

        if (Enum.IsDefined(typeof(Position), emptyTilePos - 10)) // Can the Empty Tile go up?
        {
            possibleMoves.Add((Position) emptyTilePos - 10);
        }

        if (Enum.IsDefined(typeof(Position), emptyTilePos + 1)) // Can the Empty Tile go right?
        {
            possibleMoves.Add((Position) emptyTilePos + 1);
        }

        if (Enum.IsDefined(typeof(Position), emptyTilePos + 10)) // Can the Empty Tile go down?
        {
            possibleMoves.Add((Position) emptyTilePos + 10);
        }

        return possibleMoves;

    }


    public List<Position> NewState(List<Position> oldState, Position pos) 
    {
        List<Position> newState = new List<Position>(oldState);
        
        Position emptyTilePos = newState[0];
        int posIndex = newState.IndexOf(pos);

        newState[0] = pos;
        newState[posIndex] = emptyTilePos;
        //tc.PrintBoard(newState); // For Debugging
        return newState;
    }
}
