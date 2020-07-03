using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    // Start variable values
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

        if(_gc.auxCount == -1)
        {
            board.currentState = GameState.move;
            _gc.auxCount = 0;
        }
    }

    public IEnumerator CheckMove()
    {
        _gc.auxCount = 0;
        _gc.playSwapPiecesSFX();
        yield return new WaitForSeconds(.7f);
        _gc.auxObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        if (otherChar != null)
        {
            if (!isMatched && !otherChar.GetComponent<CharController>().isMatched)
            {
                otherChar.GetComponent<CharController>().row = row;
                otherChar.GetComponent<CharController>().column = column;
                row = previousRow;
                column = previousColumn;
                _gc.playSwapPiecesSFX();
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
        if (board.currentState == GameState.move && _gc.auxCount == 0)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _gc.auxObject = this.gameObject;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

            if (_gc.auxCount == 0)
            {
                _gc.aux1 = firstTouchPosition;
                _gc.auxCount++;
            }
        }
        else if (board.currentState == GameState.selected)
        {
            _gc.aux2 = firstTouchPosition;
            _gc.auxCount--;
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            _gc.playSelectPieceSFX();
        }
        else if (board.currentState == GameState.selected)
        {
            MovePiecesSelected();
            _gc.playSelectPieceSFX();
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
            _gc.auxCount = 0;
            MovePieces();
        }
        else
            board.currentState = GameState.selected;//
    }

    void Move(Vector2 direction)
    {
        board.currentState = GameState.wait;
        _gc.auxCount = 0;
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
        {
            _gc.auxObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            _gc.auxCount = 0;
            board.currentState = GameState.move;
        }
    }

    void MovePiecesSelected()
    {
        board.currentState = GameState.wait;
        if (Mathf.Abs(_gc.aux2.y - _gc.aux1.y) > swipeResist || Mathf.Abs(_gc.aux2.x - _gc.aux1.x) > swipeResist)
        {
            int previousColumn = _gc.auxObject.GetComponent<CharController>().column;
            int previousRow = _gc.auxObject.GetComponent<CharController>().row;

            if (column - previousColumn == 1 && previousRow == row)
                Move(-Vector2.right);
            else if (column - previousColumn == -1 && previousRow == row)
                Move(-Vector2.left);
            else if (row - previousRow == 1 && previousColumn == column)
                Move(-Vector2.up);
            else if (row - previousRow == -1 && previousColumn == column)
                Move(-Vector2.down);
            else
            {
                _gc.auxObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                board.currentState = GameState.move;
            }
        }
        else
        {
            _gc.auxObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            _gc.auxCount = 0;
            board.currentState = GameState.move;
        }
    }
}