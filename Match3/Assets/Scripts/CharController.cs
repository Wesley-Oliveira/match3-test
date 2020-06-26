using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    private GameObject otherChar;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    public float swipeAngle = 0;

    void Start()
    {
        board = FindObjectOfType<Board>() as Board;
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        column = targetX;
        row = targetY;
        previousColumn = column;
        previousRow = row;
    }

    void Update()
    {
        FindMatches();

        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            /*Debug.Log("Left - " + board.allChars[column - 1, row].gameObject.tag);
                Debug.Log("Right - " + board.allChars[column + 1, row].gameObject.tag);
                Debug.Log("Up - " + board.allChars[column, row + 1]);
                Debug.Log("Down - " + board.allChars[column, row - 1]);
            */
            mySprite.color = new Color(1f, 1f, 1f, .2f);

        }

        targetX = column;
        targetY = row;

        // Horizontal movement
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allChars[column, row] = this.gameObject;
        }

        // Vertical movement
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allChars[column, row] = this.gameObject;
        }
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.3f);
        
        if(otherChar != null)
        {
            if (!isMatched && !otherChar.GetComponent<CharController>().isMatched)
            {
                otherChar.GetComponent<CharController>().row = row;
                otherChar.GetComponent<CharController>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            otherChar = null;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(
                finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x
            ) * 180 / Mathf.PI;
        MovePieces();
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right Swipe
            otherChar = board.allChars[column + 1, row];
            otherChar.GetComponent<CharController>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up Swipe
            otherChar = board.allChars[column, row + 1];
            otherChar.GetComponent<CharController>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left Swipe
            otherChar = board.allChars[column - 1, row];
            otherChar.GetComponent<CharController>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down Swipe
            otherChar = board.allChars[column, row - 1];
            otherChar.GetComponent<CharController>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftChar1 = board.allChars[column - 1, row];
            GameObject rightChar1 = board.allChars[column + 1, row];

            //Checking horizontal match 
            if (leftChar1.tag == this.gameObject.tag && rightChar1.tag == this.gameObject.tag)
            {
                leftChar1.GetComponent<CharController>().isMatched = true;
                rightChar1.GetComponent<CharController>().isMatched = true;
                isMatched = true;
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upChar1 = board.allChars[column, row + 1];
            GameObject downChar1 = board.allChars[column, row - 1];

            //Checking vertical match
            if (upChar1.tag == this.gameObject.tag && downChar1.tag == this.gameObject.tag)
            {
                upChar1.GetComponent<CharController>().isMatched = true;
                downChar1.GetComponent<CharController>().isMatched = true;
                isMatched = true;
            }
        }
    }
}
