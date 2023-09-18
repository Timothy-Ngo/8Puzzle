//-------------------------------------------------------------------
// Name: Timothy Ngo
// School Email: timothyngo@nevada.unr.edu
//-------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;


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
    public float maxMoveTime = 1f;
    float timer;
    bool activeSolver = false;
    Stack<Vector3> solutionMoves = new Stack<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Key Pressed");
            BFS(tc.gameBoard);
            ReconstructMoves(parentDict);
        }

        // Shuffle
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartShuffle();
        }

        if (doShuffle && numberOfShuffles > 0)
        {
            shuffleTimer -= Time.deltaTime;
            if (shuffleTimer <= 0)
            {
                prevVector = Shuffle(prevVector);
                shuffleTimer = timePerMove;
                numberOfShuffles -= 1;
            }
            if (numberOfShuffles <= 0)
            {
                doShuffle = false;
                Debug.Log("Finished Shuffling");
            }
        }

        // AI Solver
        if (Input.GetKeyDown(KeyCode.A))
        {
            BFSSolver();
        }

        if (activeSolver && solutionMoves.Count > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                tc.MoveEmpty(solutionMoves.Pop());
                timer = maxMoveTime;
            }
            if (solutionMoves.Count == 0)
            {
                activeSolver = false;
                CheckForGoalState();
                Debug.Log("Finished Solving");
            }
        }
    }
    //-------------------------------------------------------------------------------

    //---------------------DEBUGGING-------------------------------------------------
    public void PrintStringState(string state)
    {
        Debug.Log(state);
    }
    //-------------------------------------------------------------------------------

    //-------------BREADTH FIRST SEARCH------------------------------------------------


    public void BFSSolver()
    {
        BFS(tc.gameBoard);
        ReconstructMoves(parentDict);
        timer = maxMoveTime;
        activeSolver = true;
          
    }
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
                    
                }
            }
            

        }
        Debug.Log("Could not find solution");
        return false;
    }
  
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
    
    public string ListToString(List<Position> board)
    {
        string resultString = string.Empty;
        foreach(Position position in board)
        {
            resultString += ((int)position).ToString() + ".";
        }
        return resultString;
        
    }

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

    public void ReconstructMoves(Dictionary<string, int> parents)
    {
        Stack<Vector3> solutionPathOfMoves = new Stack<Vector3>();
        string node = ListToString(solvedState);
        string initialStringState = ListToString(tc.gameBoard);
        while (node != initialStringState)
        {
            solutionMoves.Push(FindMove(parents[node],(int)EmptyPositionFromString(node)));
            //Debug.Log(FindMove(parents[node], (int)EmptyPositionFromString(node)));
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


    //--------------------------SHUFFLE----------------------------------------------


    [Header("Shuffle Settings")]
    public bool doShuffle = false;
    public int halfMinNumberOfShuffles = 5;
    public int halfMaxNumberOfShuffles = 10;
    public int numberOfShuffles;
    public float timePerMove = 1.0f;
    public float shuffleTimer;
    private Vector3 prevVector = Vector3.zero;

    public void StartShuffle()
    {
        if (!doShuffle)
        {
            doShuffle = true;
            shuffleTimer = timePerMove;
            numberOfShuffles = UnityEngine.Random.Range(halfMinNumberOfShuffles, halfMaxNumberOfShuffles) * 2;
        }
        else
        {
            Debug.Log("Shuffle in progress");
        }
    }

    public Vector3 Shuffle(Vector3 prevVector)
    {
        
        Vector3 randomVector = RandomMove(tc.gameBoard);

        while (randomVector == (-prevVector))
        {
            randomVector = RandomMove(tc.gameBoard);
        }

        tc.MoveEmpty(randomVector);

        return randomVector;

    }

    public Vector3 RandomMove(List<Position> board)
    {
        List<Position> possibleMoves = new List<Position>();
        possibleMoves = PossibleMoves(board);
        return FindMove((int)board[0], (int)possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)]);
    }

    //-------------------------------------------------------------------------------

    [Header("End Game Add-Ons")]
    public LeanPulse leanSolved;
    public LeanPulse leanUnsolved;
    public AudioSource winSfx;
    public AudioSource lossSfx;
    public void CheckForGoalState()
    {
        if (IsGoalState(tc.gameBoard))
        {
            leanSolved.Pulse();
            winSfx.Play();
        }
        else
        {
            leanUnsolved.Pulse();
            lossSfx.Play();
        }
    }
}
