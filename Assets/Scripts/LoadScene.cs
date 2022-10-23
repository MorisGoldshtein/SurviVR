using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour{
    string[] scenes = {"Test","Test2","Fist","Dungeon", "Menu", "WildWest"};
    public Collider boxCol;

    void Update(){
        int current_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        if (Input.GetKeyDown(KeyCode.O)){
            Debug.Log("Pressed O");
            current_scene++;
            UnityEngine.SceneManagement.SceneManager.LoadScene(scenes[current_scene % scenes.Length]);
        }
        if (Input.GetKeyDown(KeyCode.L)){
            Debug.Log("Pressed L");
            if(current_scene == 0)
                current_scene = scenes.Length;
            current_scene--;
            UnityEngine.SceneManagement.SceneManager.LoadScene(scenes[current_scene % scenes.Length]);
        }
        
    }
}
