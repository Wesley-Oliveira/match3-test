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

    private int score;
    private int maxScore;
    private int round;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip sfxSwapPieces;
    public AudioClip sfxMatch;

    void Start()
    {
        PlayerPrefs.SetInt("Score", 0);

        time = 121;
        tempTime = 0f;
        barSize = 228f;
        score = 0;
        maxScore = 5;
        round = 1;

        scoreTxt.text = "Score: " + score + " / " + maxScore;
        roundTxt.text = "Round: #" + round;
    }

    void Update()
    {
        tempTime += Time.deltaTime;
        time -= Time.deltaTime;

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
            StartCoroutine("GameOver");
        }
    }

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

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("GameOver");
    }

    public void playSwapPiecesSFX()
    {
        sfxSource.PlayOneShot(sfxSwapPieces, 1f);
    }

    public void playMatchSFX()
    {
        sfxSource.PlayOneShot(sfxMatch, 1f);
    }
}