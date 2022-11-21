using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public int sceneBuildIndex;

    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("HITIT");
            GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene(NameFromIndex(sceneBuildIndex));
        }
    }
}