using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}