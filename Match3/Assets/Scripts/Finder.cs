using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private void AddToListAndMatch(GameObject char_)
    {
        if (!currentMatches.Contains(char_))
            currentMatches.Add(char_);

        char_.GetComponent<CharController>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject char1, GameObject char2, GameObject char3)
    {
        AddToListAndMatch(char1);
        AddToListAndMatch(char2);
        AddToListAndMatch(char3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentChar = board.allChars[i, j];
                if (currentChar != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftChar = board.allChars[i - 1, j];
                        GameObject rightChar = board.allChars[i + 1, j];

                        if (leftChar != null && rightChar != null)
                            if(leftChar.tag == currentChar.tag && rightChar.tag == currentChar.tag)
                                GetNearbyPieces(leftChar, currentChar, rightChar);
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upChar = board.allChars[i, j + 1];
                        GameObject downChar = board.allChars[i, j - 1];

                        if (upChar != null && downChar != null)
                            if (upChar.tag == currentChar.tag && downChar.tag == currentChar.tag)
                                GetNearbyPieces(upChar, currentChar, downChar);                         
                    }
                }
            }
        }
    }
}
