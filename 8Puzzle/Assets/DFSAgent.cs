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
            DFS(tc.currentState);
        }
        
    }

    public void DFS(GameState initialState) // I'm going to make the states be the actions that get to them
    {
        Stack<GameState> stack = new Stack<GameState>(); 
        stack.Push(initialState);

        HashSet<Dictionary<int,int>> visited = new HashSet<Dictionary<int,int>>();

        Dictionary<GameState,GameState> parent = new Dictionary<GameState,GameState>(); // <current state, parent state> Not sure if I should make it <GameState, move used to achieve this state>
        parent[initialState] = null;

        
        while (stack.Count > 0)
        {
            GameState current = stack.Pop();
            if (!visited.Contains(current.gameBoard)) // Not sure if hashset will be able to use Contains on a custom class
            {
                Debug.Log("Passed Contains 1");
                if (current.IsGoalState()) // I'm assuming this is the visit function
                {
                    Debug.Log("Reached Goal State");
                    foreach (int position in current.matrixPositions)
                    {
                        //Debug.Log("Solution: " + current.gameBoard[position]);
                        current.PrintGameBoardData();
                    }
                }
                visited.Add(current.gameBoard); // Visited is not working correctly
                foreach (Dictionary<int, int> state in visited)
                {
                    Debug.Log("----------------Visited------------");
                    GameState a = new GameState(state);
                    a.PrintGameBoardData();
                }
                //Debug.Log(visited.Count);
                foreach (Vector3 move in current.PossibleMoves())
                {
                    GameState newState = new GameState(current.ApplyMove(move));
                    //newState.PrintGameBoardData();

                    
                    if (!visited.Contains(newState.gameBoard))
                    {
                        Debug.Log("Check1");
                        Debug.Log(stack.Count);
                        stack.Push(newState);
                        Debug.Log(stack.Count);
                    }
                    
                }
            }

        }
    }
    

}


