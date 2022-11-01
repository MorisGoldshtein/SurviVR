using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("quitted");
    }

    public void PlayLucas()
    {
        SceneManager.LoadScene("WildWest");
    }

    public void PlayDillon()
    {
        SceneManager.LoadScene("Enemy AI Testing");
    }

    public void PlayKen()
    {
        SceneManager.LoadScene("Dmg Kill");
    }

    public void PlayMoris()
    {
        SceneManager.LoadScene("Obj Grab");
    }
}
