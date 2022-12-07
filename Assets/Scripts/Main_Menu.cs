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

    //Add current scene to a list of past levels accessed
    public void PlayLucas()
    {
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("WildWest");
    }

    public void PlayDillon()
    {
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("Restaurant Brawl");
    }

    public void PlayKen()
    {
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("Dmg Kill");
    }

    public void PlayMoris()
    {
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("Obj Grab");
    }

    public void RetryMenu()
    {
        GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().PreviousScene();
    }
}
