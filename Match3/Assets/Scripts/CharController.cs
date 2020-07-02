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
    public float swipeResist = 1f;

    private Finder finderMatches;
    private _GC _gc;

    void Start()
    {
        board = FindObjectOfType<Board>() as Board;
        finderMatches = FindObjectOfType<Finder>() as Finder;
        _gc = FindObjectOfType(typeof(_GC)) as _GC;
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

            if (board.allChars[column, row] != this.gameObject)
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
        }
        board.currentState = GameState.move;
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*else if(board.currentState == GameState.selected)
        {

        }*/
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            _gc.playSwapPiecesSFX();
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
            board.currentState = GameState.wait;
            MovePieces();
        }
        else
        {
            /*//condition here
            board.currentState = GameState.selected;
            */
            board.currentState = GameState.move;
        }
    }

    void Move(Vector2 direction)
    {
        otherChar = board.allChars[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;

        otherChar.GetComponent<CharController>().column += -1 * (int)direction.x;
        otherChar.GetComponent<CharController>().row += -1 * (int)direction.y;
        column += (int)direction.x;
        row += (int)direction.y;
        StartCoroutine(CheckMove());
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
            Move(Vector2.right);
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
            Move(Vector2.up);
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            Move(Vector2.left);
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
            Move(Vector2.down);        
        else
            board.currentState = GameState.move;
    }
}