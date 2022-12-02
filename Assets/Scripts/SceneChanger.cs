using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//https://forum.unity.com/threads/how-can-i-open-previous-scene.652507/

public class SceneChanger : MonoBehaviour
{

    private List<string> sceneHistory = new List<string>();  //running history of scenes
                                                             //The last string in the list is always the current scene running

    void Start()
    {
        sceneHistory.Add(SceneManager.GetActiveScene().name);
        DontDestroyOnLoad(this.gameObject);  //Allow this object to persist between scene changes
    }

    //add the scene to the sceneHistory list
    public void LoadScene(string newScene)
    {
        sceneHistory.Add(newScene);
        SceneManager.LoadScene(newScene);
    }

    //load the previous scene
    public bool PreviousScene()
    {
        bool returnValue = false;
        if (sceneHistory.Count >= 2)  //Checking that we have actually switched scenes enough to go back to a previous scene
        {
            returnValue = true;
            sceneHistory.RemoveAt(sceneHistory.Count - 1);
            SceneManager.LoadScene(sceneHistory[sceneHistory.Count - 1]);
        }

        return returnValue;
    }

}

