using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static int score;
    public TMP_Text tm;

    void Start()
    {
        score = 0;
    }

    public void updateScore(int num)
    {
        score += num;
        tm.SetText(score.ToString());
    }
}
