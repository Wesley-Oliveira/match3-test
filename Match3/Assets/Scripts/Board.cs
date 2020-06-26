using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                int dotToUse = Random.Range(0, chars.Length);
                GameObject dot = Instantiate(chars[dotToUse], tempPosition, Quaternion.identity) as GameObject;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + ") - CH";

                allChars[i, j] = dot;
            }
        }
    }
}
