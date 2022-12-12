using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSwords : MonoBehaviour
{
    Vector3 sword_spawn_position;
    /*
        Sometimes, a double collision occurs -> two swords spawned
        instead of one.
        Solution: a bool that is meant to allow entry to spawn logic
        that will be turned false after entry which prevents second
        collision from causing spawn. When sword respawns, its bool
        will be true so nothing is broken via this method.
    */
    bool allow = true;

    // Start is called before the first frame update
    void Start()
    {
        sword_spawn_position = gameObject.transform.position;
    }
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("COLLIDECOLLIDECOLLIDECOLLIDE");
        if (other.gameObject.name == "Arena" && allow){
            allow = !allow;
            Debug.Log("TELEPORT");
            Rigidbody sword_rbody = gameObject.GetComponent<Rigidbody>();
            sword_rbody.velocity = new Vector3(0,0,0);
            gameObject.transform.rotation = new Quaternion(0,0,0,0);
            //gameObject.transform.position = new Vector3(33.8f,5f,1.4f);
            gameObject.transform.position = sword_spawn_position;
            Instantiate(gameObject,sword_spawn_position,new Quaternion(0,90,0,0));
            Destroy(gameObject);
        }
    }
}
