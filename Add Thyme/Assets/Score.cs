using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static int score = 0;
    public Text ScoreTxt;

    private void Start()
    {
        ScoreTxt.text = "Combo: 0";
    }

    public void IncrementScore()
    {
        score++;

        ScoreTxt.text = "Combo: " + score.ToString();
        PlayerPrefs.SetInt("ScoreToUpdate", PlayerPrefs.GetInt("ScoreToUpdate", 0) + 1);

        Debug.Log("SCORE: " + PlayerPrefs.GetInt("ScoreToUpdate"));
    }

    public void ResetScore()
    {
        score = 0;

        ScoreTxt.text = "Combo: 0";
        PlayerPrefs.SetInt("ScoreToUpdate", 0);
    }

    public int GetScore()
    {
        return score;
    }
}
