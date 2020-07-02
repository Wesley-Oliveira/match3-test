using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;

    // Start is called before the first frame update
    void Start()
    {
        score.text = "SCORE - " + PlayerPrefs.GetInt("Score").ToString();
    }
}
