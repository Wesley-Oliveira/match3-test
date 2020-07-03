using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;

    // Used for set the final score on the scene game over
    void Start()
    {
        score.text = "SCORE - " + PlayerPrefs.GetInt("Score").ToString();
    }
}
