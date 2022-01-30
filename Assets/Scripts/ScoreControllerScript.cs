using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreControllerScript : MonoBehaviour {

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] float scoreMultiplier;


    static float mStartPos;
    public float startPos { get { return mStartPos; } set { mStartPos = value; } }

    

    void Start ()
    {
        UpdateScore();
	}

    private void Update()
    {
        GlobalState.state.score = (int)((transform.position.y - startPos) * scoreMultiplier);
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + GlobalState.state.score;
    }
}