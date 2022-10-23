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
        SceneManager.LoadScene("Test");
    }

    public void PlayKen()
    {
        SceneManager.LoadScene("Fist");
    }

    public void PlayMorris()
    {
        SceneManager.LoadScene("Dungeon");
    }
}
