using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Move
{
    up, down, left, right
}

public class BFSAgent : MonoBehaviour
{
    public TileController tc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            List<Position> newState = new List<Position>();
            foreach (Position position in PossibleMoves(tc.gameBoard))
            {
                newState = NewState(tc.gameBoard, position);
                //PrintBoard(newState);
            }
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

        if (Enum.IsDefined(typeof(Position), emptyTilePos + 1)) // Can the Empty Tile go right?
        {
            possibleMoves.Add((Position) emptyTilePos + 1);
        }

        if (Enum.IsDefined(typeof(Position), emptyTilePos - 10)) // Can the Empty Tile go up?
        {
            possibleMoves.Add((Position) emptyTilePos - 10);
        }

        if (Enum.IsDefined(typeof(Position), emptyTilePos + 10)) // Can the Empty Tile go down?
        {
            possibleMoves.Add((Position) emptyTilePos + 10);
        }

        return possibleMoves;

    }

    public List<Position> NewState(List<Position> oldState, Position pos) 
    {
        List<Position> newState = oldState;
        
        Position emptyTilePos = newState[0];
        int posIndex = newState.IndexOf(pos);

        newState[0] = pos;
        newState[posIndex] = emptyTilePos;
        //tc.PrintBoard(newState); // For Debugging
        return newState;
    }
}
