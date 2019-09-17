using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSBNetwork;

public class ScoreSign : MonoBehaviour
{   
    public Text score;

    public enum Team { Blue, Red }   
    public Team team;

    public int currentScore;
    public int kill;
    public int death;

    private void Start()
    {       
        currentScore = 0;
        kill = 0;
        death = 0;
        UpdateScore(currentScore);
    }

    public void UpdateScore(int _score)
    {
        currentScore = _score;
        score.text = currentScore.ToString();
    }
}
