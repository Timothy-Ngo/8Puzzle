using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;




public class DFSAgent : MonoBehaviour
{
    //public GameState gs = new GameState();

    public TileController tc;
    // Start is called before the first frame update
    void Start()
    {

    }
   
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //tc.currentState.PrintGameBoardData();

            Debug.Log("Key Pressed");
            //DFS(tc.currentState.gameBoard);
            BFS(tc.currentState.gameBoard);
        }
        
    }

    public List<int> matrixPositions = new List<int> {
        0, 1, 2,
        10, 11, 12,
        20, 21, 22
    };

    public bool IsGoalState(Dictionary<int,int> gameBoard) // Not Tested
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
    public int GetEmptyMatrixPosition(Dictionary<int, int> gameBoard) // Can be optimized
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
    public List<Vector3> PossibleMoves(Dictionary<int,int> board) 
    {
        int emptyPos = GetEmptyMatrixPosition(board);
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
    public void BFS(Dictionary<int,int> initialBoard)
    {
        Queue<Dictionary<int,int>> queue = new Queue<Dictionary<int,int>>();

        queue.Enqueue(initialBoard);
        //initialState.PrintGameBoardData();

        HashSet<Dictionary<int, int>> visited = new HashSet<Dictionary<int, int>>();

        //Dictionary<GameState, GameState> parent = new Dictionary<GameState, GameState>(); // <current state, parent state> Not sure if I should make it <GameState, move used to achieve this state>
        //parent[initialState] = null;

        
        while (queue.Count > 0)
        {
            //Debug.Log(queue.Count);
            Dictionary<int,int> currentBoard = new Dictionary<int,int>(queue.Dequeue()) ;
            //current.PrintGameBoardData();
            if (!visited.Contains(currentBoard))
            {
                //Debug.Log("Passed Contains 1");
                if (IsGoalState(currentBoard)) // I'm assuming this is the visit function
                {
                    Debug.Log(queue.Count);
                    Debug.Log("REACHED GOAL STATE");
                    Debug.Log(visited.Count);
                    //current.PrintGameBoardData();
                    break;
                }
                visited.Add(currentBoard);
                
                                                //current.PrintGameBoardData();
                                                //Debug.Log("visited.count: " + visited.Count);
                /*
                foreach (Dictionary<int, int> state in visited)
                {
                    Debug.Log("----------------Visited------------");
                    GameState a = new GameState(state);
                    a.PrintGameBoardData();
                }
                */
                //Debug.Log(visited.Count);
                GameState newState;
                foreach (Vector3 move in PossibleMoves(currentBoard))
                {
                    newState = new GameState(ApplyMove(move, currentBoard)); // ApplyMove should not change current
                                                                        //newState.PrintGameBoardData();

                    //Debug.Log(!visited.Contains(newState.gameBoard));
                    if (!visited.Contains(newState.gameBoard))
                    {
                        //Debug.Log("Check1");
                        //Debug.Log(stack.Count);
                        queue.Enqueue(newState.gameBoard);
                        //Debug.Log(stack.Count);
                    }

                }
                

            }
            /*
            if (queue.Count > 14000000)
            {
                Debug.Log(queue.Count);
                Debug.Log(visited.Count);
                break;
            }
            */

        }
    }

    
    public void DFS(Dictionary<int,int> initialBoard) // I'm going to make the states be the actions that get to them
    {
        Stack<Dictionary<int, int>> stack = new Stack<Dictionary<int, int>>(); 
        stack.Push(initialBoard);
        //initialState.PrintGameBoardData();

        HashSet<Dictionary<int,int>> visited = new HashSet<Dictionary<int,int>>();

        //Dictionary<GameState,GameState> parent = new Dictionary<GameState,GameState>(); // <current state, parent state> Not sure if I should make it <GameState, move used to achieve this state>
        //parent[initialState] = null;

        
        while (stack.Count > 0)
        {
            Dictionary<int,int> current = new Dictionary<int,int>(stack.Pop());
            //current.PrintGameBoardData();
           
            if (!visited.Contains(current)) 
            {
                //Debug.Log("Passed Contains 1");
                if (IsGoalState(current)) // I'm assuming this is the visit function
                {
                    Debug.Log(stack.Count);
                    Debug.Log("REACHED GOAL STATE");
                    Debug.Log(visited.Count);
                    //current.PrintGameBoardData();
                    /*
                    foreach (int position in current.matrixPositions)
                    {
                        //Debug.Log("Solution: " + current.gameBoard[position]);
                    }
                    */
                    break;
                }
                visited.Add(current); 
                //current.PrintGameBoardData();
                //Debug.Log("visited.count: " + visited.Count);
                /*
                foreach (Dictionary<int, int> state in visited)
                {
                    Debug.Log("----------------Visited------------");
                    GameState a = new GameState(state);
                    a.PrintGameBoardData();
                }
                */
                //Debug.Log(visited.Count);
                GameState newState;
                foreach (Vector3 move in PossibleMoves(current))
                {
                    newState = new GameState(ApplyMove(move, current )); // ApplyMove should not change current
                    //newState.PrintGameBoardData();

                    //Debug.Log(!visited.Contains(newState.gameBoard));
                    if (!visited.Contains(newState.gameBoard))
                    {
                        //Debug.Log("Check1");
                        //Debug.Log(stack.Count);
                        stack.Push(newState.gameBoard);
                        //Debug.Log(stack.Count);
                    }
                    
                }
                
                
            }

            if (stack.Count > 300000)
            {
                Debug.Log(stack.Count);
                break;
            }
        }
    }

    public Dictionary<int, int> ApplyMove(Vector3 move, Dictionary<int,int> board) // Not Tested
    {
        //GameState newState = new GameState(state);
        Dictionary<int, int> newBoard = new Dictionary<int, int>(board);
        int emptyPos = GetEmptyMatrixPosition(newBoard);
        int newPos = 0; // If there are no possible moves then nothing happens to the board

        if (move == Vector3.up)
        {
            newPos = emptyPos - 10;
        }
        else if (move == Vector3.down)
        {
            newPos = emptyPos + 10;
        }
        else if (move == Vector3.left)
        {
            newPos = emptyPos - 1;
        }
        else if (move == Vector3.right)
        {
            newPos = emptyPos + 1;
        }
        else
        {
            Debug.Log("Error: No Possible Movement");
        }

        SwapTilePositions(emptyPos, newPos, newBoard);
        return newBoard;
    }
    public void SwapTilePositions(int pos1, int pos2, Dictionary<int,int> gameBoard)
    {
        int temp = gameBoard[pos1];
        gameBoard[pos1] = gameBoard[pos2];
        gameBoard[pos2] = temp;
    }


}


