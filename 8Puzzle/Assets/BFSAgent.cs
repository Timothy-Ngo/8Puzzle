using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

enum Move
{
    up, down, left, right
}

public class BFSAgent : MonoBehaviour
{
    public TileController tc;

    [Header("BFS Solver")]
    public float maxMoveTime = 0.5f;
    public float currentMoveTime;

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
        //List<Vector3> solvedMoves = ReconstructMoves(BFS(tc.gameBoard));
    }

    //-------------------------------------------------------------------------------
    public List<Vector3> ReconstructMoves(Dictionary<List<Position>, List<Position>> parents)
    {
        return null;
    }

    //-------------------------------------------------------------------------------
    public bool BFS(List<Position> initialState)
    {
        Queue<List<Position>> frontier = new Queue<List<Position>>();
        HashSet<string> stringFrontier = new HashSet<string>(); 
        HashSet<string> explored = new HashSet<string>();
        //Dictionary<List<Position>, List<Position>> parents = new Dictionary<List<Position>, List<Position>>();
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
                //parents[newState] = currentState;
                if (!explored.Contains(newStringState) || !stringFrontier.Contains(newStringState))
                {
                    if (IsGoalState(newState))
                    {
                        Debug.Log("GOAL STATE FOUND");
                        return true; 
                    }
                    frontier.Enqueue(newState);
                    //Debug.Log("Added");
                    //Debug.Log(frontier.Count);
                }
            }


        }
        return false;

    }


    public string ListToString(List<Position> board)
    {
        string resultString = string.Empty;
        foreach(Position position in board)
        {
            resultString += ((int)position).ToString();
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
