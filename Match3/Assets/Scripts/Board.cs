using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <Refactor>
/// Procurar uma solução mais otimizada para trabalhar com matrizes
/// </Refactor>

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
    public GameObject tilePrefab;
    public GameObject[] chars;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allChars;
    public GameObject destroyEffect;

    private Finder findMatches;
    void Start()
    {
        findMatches = FindObjectOfType<Finder>() as Finder;
        allTiles = new BackgroundTile[width, height];
        allChars = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ") - BG";

                int charToUse = Random.Range(0, chars.Length);

                int maxIterantions = 0;
                while(MatchesAt(i, j, chars[charToUse]) && maxIterantions < 100)
                {
                    charToUse = Random.Range(0, chars.Length);
                    maxIterantions++;
                    Debug.Log(maxIterantions);
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

    private void RefilBoard()
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
        RefilBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }

        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}
