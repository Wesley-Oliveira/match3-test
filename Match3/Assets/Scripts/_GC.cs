using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _GC : MonoBehaviour
{
    public Text timeTxt;
    public Text scoreTxt;
    public Text roundTxt;
    public GameObject barTime;
    public int count;
    private float time;
    private float tempTime;
    private float barSize;
    private int aux;

    private float verify;
    private Board board;

    private int score;
    private int maxScore;
    private int round;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip sfxSwapPieces;
    public AudioClip sfxSelectPiece;
    public AudioClip sfxMatch;

    [Header("Previous Object")]
    public Vector2 aux1;
    public Vector2 aux2;
    public int auxCount;
    public GameObject auxObject;

    // Start variable values
    void Start()
    {
        PlayerPrefs.SetInt("Score", 0);
        verify = 0;
        time = 121;
        tempTime = 0f;
        barSize = 228f;
        score = 0;
        maxScore = 5;
        round = 1;

        scoreTxt.text = "Score: " + score + " / " + maxScore;
        roundTxt.text = "Round: #" + round;

        board = FindObjectOfType<Board>() as Board;
    }

    void Update()
    {
        tempTime += Time.deltaTime;
        time -= Time.deltaTime;

        // Timer count and decrement TimeBar
        if (tempTime <= 120)
        {
            aux = (int)time;
            timeTxt.text = aux.ToString();
            barSize = 228f - ((tempTime / 120) * 228f);
            barTime.transform.localScale = new Vector3(barSize, 117f, 117f);
        }
        else
        {
            timeTxt.text = "0";
            PlayerPrefs.SetInt("Score", round);
            StartCoroutine(GameOver());
        }

        // Verify deadstate and unlock
        if(board.currentState == GameState.wait)
        {
            verify += Time.deltaTime;
            if (verify >= 3.5f)
            {
                board.currentState = GameState.move;
                aux1 = Vector2.zero;
                aux2 = Vector2.zero;
                auxCount = 0;
                auxObject = null;
                verify = 0;
            }
        }
        else
        {
            verify = 0;
        }
    }

    // Score controller
    public void Scored(int countPieces)
    {
        if(score < maxScore - 1)
        {
            if (countPieces > 2 && countPieces < 9)
            {
                score++;
                scoreTxt.text = "Score: " + score + " / " + maxScore;
                count = 0;
            }
        }
        else
        {
            maxScore += 5;
            round += 1;
            score = 0;
            time = 120;
            tempTime = 0;
            roundTxt.text = "Round: #" + round;
            scoreTxt.text = "Score: " + score + " / " + maxScore;
            barSize = 158f;
        }
    }

    // Coroutine for load a gameover scene
    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("GameOver");
    }

    // Play a specific sfx
    public void playSwapPiecesSFX()
    {
        sfxSource.PlayOneShot(sfxSwapPieces, 1f);
    }
    public void playSelectPieceSFX()
    {
        sfxSource.PlayOneShot(sfxSelectPiece, 1f);
    }
    public void playMatchSFX()
    {
        sfxSource.PlayOneShot(sfxMatch, 1f);
    }
}