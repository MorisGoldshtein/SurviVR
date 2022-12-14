using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static int score;
    public TMP_Text tm;
    public Animator cheerleader;

    void Start()
    {
        score = 0;
    }

    public void updateScore(int num)
    {
        score += num;

        if (score % 50 == 0)
        {
            cheerleader.SetBool("score50", true);
        }
        else
        {
            cheerleader.SetBool("score50", false);
        }

        tm.SetText(score.ToString());
    }
}
