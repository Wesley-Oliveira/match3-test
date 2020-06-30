using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <BugList>
///
/// </BugList>
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
    public float swipeResist = 1f;

    private Finder finderMatches;

    void Start()
    {
        board = FindObjectOfType<Board>() as Board;
        finderMatches = FindObjectOfType<Finder>() as Finder;
    }

    void Update()
    {
        targetX = column;
        targetY = row;

        // Horizontal movement
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            
            if(board.allChars[column, row] != this.gameObject)
                board.allChars[column, row] = this.gameObject;

            finderMatches.FindAllMatches();
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
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);

            if (board.allChars[column, row] != this.gameObject)
                board.allChars[column, row] = this.gameObject;

            finderMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);

        if (otherChar != null)
        {
            if (!isMatched && !otherChar.GetComponent<CharController>().isMatched)
            {
                otherChar.GetComponent<CharController>().row = row;
                otherChar.GetComponent<CharController>().column = column;
                row = previousRow;
                column = previousColumn;

                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherChar = null;
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)        
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(
                finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x
            ) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right Swipe
            otherChar = board.allChars[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherChar.GetComponent<CharController>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up Swipe
            otherChar = board.allChars[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherChar.GetComponent<CharController>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left Swipe
            otherChar = board.allChars[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherChar.GetComponent<CharController>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down Swipe
            otherChar = board.allChars[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherChar.GetComponent<CharController>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }
}