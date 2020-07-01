using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject[] chars;
    private bool[,] blankSpaces;

    public GameObject[,] allChars;
    public GameObject destroyEffect;

    private Finder findMatches;
    private _GC _gc;

    void Start()
    {
        findMatches = FindObjectOfType<Finder>() as Finder;
        _gc = FindObjectOfType(typeof(_GC)) as _GC;
        allChars = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                int charToUse = Random.Range(0, chars.Length);

                int maxIterantions = 0;
                while(MatchesAt(i, j, chars[charToUse]) && maxIterantions < 100)
                {
                    charToUse = Random.Range(0, chars.Length);
                    maxIterantions++;
                }
                maxIterantions = 0;

                GameObject char_ = Instantiate(chars[charToUse], tempPosition, Quaternion.identity) as GameObject;
                char_.GetComponent<CharController>().row = j;
                char_.GetComponent<CharController>().column = i;
                char_.transform.parent = this.transform;
                char_.name = "(" + i + ", " + j + ") - CH";

                allChars[i, j] = char_;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allChars[column - 1, row].tag == piece.tag && allChars[column - 2, row].tag == piece.tag)
                return true;
            if (allChars[column, row - 1].tag == piece.tag && allChars[column, row - 2].tag == piece.tag)
                return true;
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
                if (allChars[column, row - 1].tag == piece.tag && allChars[column, row - 2].tag == piece.tag)
                    return true;
            if(column > 1)
                if (allChars[column - 1, row].tag == piece.tag && allChars[column - 2, row].tag == piece.tag)
                    return true;
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allChars[column, row].GetComponent<CharController>().isMatched)
        {
            Instantiate(destroyEffect, allChars[column, row].transform.position, Quaternion.identity);
            Destroy(allChars[column, row]);
            _gc.count++;
            _gc.Scored(_gc.count);
            allChars[column, row] = null;
            
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if(allChars[i, j] != null)
                    DestroyMatchesAt(i, j);
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] == null)
                    nullCount++;
                else if (nullCount > 0)
                {
                    allChars[i, j].GetComponent<CharController>().row -= nullCount;
                    allChars[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int charToUse = Random.Range(0, chars.Length);
                    GameObject piece = Instantiate(chars[charToUse], tempPosition, Quaternion.identity);
                    piece.transform.parent = this.transform;
                    piece.name = tempPosition + " - NewCH";
                    allChars[i, j] = piece;
                    piece.GetComponent<CharController>().row = j;
                    piece.GetComponent<CharController>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] != null)
                {
                    if (allChars[i, j].GetComponent<CharController>().isMatched)
                        return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        yield return new WaitForSeconds(.5f);

        if (IsDeadLocked())
        {
            Debug.Log("Deadlocked!!");
            ShufflelBoard();
        }

        currentState = GameState.move;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //Take the second piece and save it in a holder
        GameObject holder = allChars[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //switching the first char to be the second position
        allChars[column + (int)direction.x, row + (int)direction.y] = allChars[column, row];
        //Set the first char to be  the second char
        allChars[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] != null)
                {
                    //Make sure that one and two to the right are in the board
                    if (i < width - 2) //
                    {
                        //Check if the chars to the right and two to the right exist
                        if (allChars[i + 1, j] != null && allChars[i + 2, j] != null)
                        {
                            if (allChars[i + 1, j].tag == allChars[i, j].tag && allChars[i + 2, j].tag == allChars[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    
                    if (j < height - 2)//
                    {
                        //Check if the chars above exist
                        if (allChars[i, j + 1] != null && allChars[i, j + 2] != null)
                        {
                            if (allChars[i, j + 1].tag == allChars[i, j].tag && allChars[i, j + 2].tag == allChars[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allChars[i, j] != null)
                {
                    if(i< width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShufflelBoard()
    {
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();

        //add every pieces to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] != null)
                {
                    newBoard.Add(allChars[i, j]);
                }
            }
        }

        //for every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i, j])
                {
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);
                                       
                    int maxIterantions = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterantions < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterantions++;
                    }
                    //Make a container for the piece
                    CharController piece = newBoard[pieceToUse].GetComponent<CharController>();
                    maxIterantions = 0;

                    //Assign the column to the piece
                    piece.column = i;
                    //assign the row to the piece
                    piece.row = j;
                    //Fill in the chars array with this new piece
                    allChars[i, j] = newBoard[pieceToUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //Check if it's still deadlocked
        if (IsDeadLocked())
        {
            ShufflelBoard();
        }
    }
}
