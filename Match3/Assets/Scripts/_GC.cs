using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

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


    void Start()
    {
        time = 121;
        tempTime = 0f;
        barSize = 2.4f;
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
            barSize = 2.4f - ((tempTime / 120) * 2.4f);
            barTime.transform.localScale = new Vector3(barSize, 1.2388f, 1.2388f);
        }
        else
        {
            Debug.Log("Game over");
        }
    }

    public void Scored(int countPieces)
    {
        if(score < maxScore)
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
        }
    }


}