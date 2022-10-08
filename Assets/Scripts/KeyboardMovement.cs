using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class KeyboardMovement : MonoBehaviour{
    
    void Update(){
        //Value changes when A/D/Leftarrow/Rightarrow pressed -> between -1 and 1
        float horizontalInput = Input.GetAxis("Horizontal");
        //Value changes when W/S/UpArrow/DownArrow pressed -> between -1 and 1
        float verticalInput = Input.GetAxis("Vertical");

        // Combine the inputs together into a vector
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        
        //Makes diagonal movement not go faster than going vertical/horizontal only
        movementDirection.Normalize(); 

        // Apply the vector for movement
        transform.Translate(movementDirection * 5 * Time.deltaTime);


        /*
            NOTE: LETTING GO OF KEYS DOESNT STOP MOVEMENT RIGHT AWAY. TO CHANGE THIS:
            Project settings -> Input Manager -> Axes -> Horizontal -> Gravity -> Change to 18
            DO THE SAME FOR Vertical in Axes
        */
    }
}
