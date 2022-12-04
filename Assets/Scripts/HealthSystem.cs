using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    public static int health;
    public TMP_Text tm;

    void Start()
    {
        health = 0;
    }

    public void updateHealth(int num)
    {
        health += num;
        tm.SetText(health.ToString());
    }
}
