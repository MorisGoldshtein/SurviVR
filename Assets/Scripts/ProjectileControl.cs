using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
    public float bulletLife;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // CollisionDetected();
    }

    void OnTriggerEnter()
    {
        Destroy(gameObject,bulletLife); //pretty sure this will destroy it after x seconds.
    }

    // void CollisionDetected(Collider collision)
    // {
    //     if (collision.gameObject)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}


