using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    selected
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

    // Start variable values
    void Start()
    {
        findMatches = FindObjectOfType(typeof(Finder)) as Finder;
        _gc = FindObjectOfType(typeof(_GC)) as _GC;
        allChars = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        SetUp();
    }

    // Construindo o tabuleiro e adicionando personagens
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

    // Verifica se existem 3 personagens do mesmo tipo em uma mesma linha ou coluna
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
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

    // Realiza a destruição de personagens que fizeram um match, instancia efeito e adiciona pontuação
    private void DestroyMatchesAt(int column, int row)
    {
        if (allChars[column, row].GetComponent<CharController>().isMatched)
        {
            Instantiate(destroyEffect, allChars[column, row].transform.position, Quaternion.identity);
            _gc.count++;
            _gc.Scored(_gc.count);
            Destroy(allChars[column, row]);
            _gc.playMatchSFX();
            allChars[column, row] = null;            
        }
    }

    // Faz a validação de campo e se for válido, passa a posição para a função que destrói o objeto. Também inicializa coroutine para adicionar decrementar uma determinada posição da linha
    public void DestroyMatches()
    {
        currentState = GameState.wait;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if(allChars[i, j] != null)
                    DestroyMatchesAt(i, j);
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRow());
    }

    // Realiza as verificações necessárias para reduzir uma linha e inicializa a coroutine responsável por gerar novos personagens adicionando no tabuleiro e validando
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
        yield return new WaitForSeconds(.5f);
        StartCoroutine(FillBoard());
    }

    // Spawna novos personagens onde existir vaga no tabuleiro
    private void RefillBoard()
    {
        currentState = GameState.wait;
        _gc.auxCount = 0;
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

    // Valida se existe um match, retornando true ou false
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] != null)
                    if (allChars[i, j].GetComponent<CharController>().isMatched)
                        return true;
            }
        }
        return false;
    }

    // Responsável por preencher o tabuleiro, verifica deadlock, 
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
            currentState = GameState.wait;
            ShuffleBoard();
            _gc.aux1 = Vector2.zero;
            _gc.aux2 = Vector2.zero;
            _gc.auxCount = 0;
            _gc.auxObject = null;
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

    // Verifica se existe um match entre 3 objetos horizontal ou vertical
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChars[i, j] != null)
                {
                    //Make sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        //Check if the chars to the right and two to the right exist
                        if (allChars[i + 1, j] != null && allChars[i + 2, j] != null)
                            if (allChars[i + 1, j].tag == allChars[i, j].tag && allChars[i + 2, j].tag == allChars[i, j].tag)
                                return true;
                    }
                    
                    if (j < height - 2)
                    {
                        //Check if the chars above exist
                        if (allChars[i, j + 1] != null && allChars[i, j + 2] != null)
                            if (allChars[i, j + 1].tag == allChars[i, j].tag && allChars[i, j + 2].tag == allChars[i, j].tag)
                                return true;
                    }
                }
            }
        }
        return false;
    }

    // Realiza a mudança de personagem de local e verifica se houve um match
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

    // Realiza a verificação se existe possibilidade de realização de um match
    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allChars[i, j] != null)
                {
                    if(i< width - 1)
                        if (SwitchAndCheck(i, j, Vector2.right))
                            return false;

                    if (j < height - 1)
                        if (SwitchAndCheck(i, j, Vector2.up))
                            return false;
                }
            }
        }
        return true;
    }

    // Embaralha o tabuleiro
    private void ShuffleBoard()
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
            ShuffleBoard();
        }
    }

}
