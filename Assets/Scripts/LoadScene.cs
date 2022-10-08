using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour{
    void Update(){
        if (Input.GetKeyDown(KeyCode.O)){
            Debug.Log("Pressed O");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
        }
        if (Input.GetKeyDown(KeyCode.L)){
            Debug.Log("Pressed L");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Test2");
        }
    }
}
