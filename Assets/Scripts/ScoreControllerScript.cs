using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreControllerScript : MonoBehaviour {

    static Text scoreText;

    static int score;

    static float mStartPos;
    public static float startPos { get { return mStartPos; } set { mStartPos = value; } }

    private void Awake()
    {
        scoreText = GetComponent<Text>();
    }

    void Start ()
    {
        score = 0;
        UpdateScore();
	}

    public static void addScore(float addedScore)
    {
        score = (int)((addedScore - startPos) * 10);
        UpdateScore();
    }

    static void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }
}
