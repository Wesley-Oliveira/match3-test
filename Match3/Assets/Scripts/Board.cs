using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <Refactor>
/// Procurar uma solução mais otimizada para trabalhar com matrizes
/// Preocupação DESEMPENHO
/// </Refactor>
public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] chars;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allChars;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allChars = new GameObject[width, height];
        SetUp();
    }

    // refactor this code block
    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + ") - BG";

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
                char_.transform.parent = this.transform;
                char_.name = "( " + i + ", " + j + ") - CH";

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
    }
}
