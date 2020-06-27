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

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentChar = board.allChars[i, j];

                if(currentChar != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftChar = board.allChars[i - 1, j];
                        GameObject rightChar = board.allChars[i + 1, j];

                        if(leftChar != null && rightChar != null)
                        {
                            if(leftChar.tag == currentChar.tag && rightChar.tag == currentChar.tag)
                            {
                                if (!currentMatches.Contains(leftChar))
                                {
                                    currentMatches.Add(leftChar);
                                }
                                leftChar.GetComponent<CharController>().isMatched = true;
                                
                                if (!currentMatches.Contains(rightChar))
                                {
                                    currentMatches.Add(rightChar);
                                }
                                rightChar.GetComponent<CharController>().isMatched = true;

                                if (!currentMatches.Contains(currentChar))
                                {
                                    currentMatches.Add(currentChar);
                                }
                                currentChar.GetComponent<CharController>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upChar = board.allChars[i, j + 1];
                        GameObject downChar = board.allChars[i, j - 1];

                        if (upChar != null && downChar != null)
                        {
                            if (upChar.tag == currentChar.tag && downChar.tag == currentChar.tag)
                            {
                                if (!currentMatches.Contains(upChar))
                                {
                                    currentMatches.Add(upChar);
                                }
                                upChar.GetComponent<CharController>().isMatched = true;

                                if (!currentMatches.Contains(downChar))
                                {
                                    currentMatches.Add(downChar);
                                }
                                downChar.GetComponent<CharController>().isMatched = true;

                                if (!currentMatches.Contains(currentChar))
                                {
                                    currentMatches.Add(currentChar);
                                }
                                currentChar.GetComponent<CharController>().isMatched = true;
                            }

                        }
                    }
                }
            }
        }
    }
}
