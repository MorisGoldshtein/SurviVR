using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaController : MonoBehaviour
{
    // use to find the distance from gameObject to ground to check if gameObject is too far from ground. If so, destroy the gameObject
    float distToGroundLimit = 40f;

    Vector3 target;
    float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        //int x = Random.Range(0, 0);
        //int z = Random.Range(0, 0);
        //SetNewTarget(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z));
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 movementDirection = target - transform.position;
        Vector3 movementDirection = new Vector3(0,0,0);
        transform.Translate(movementDirection.normalized * speed * Time.deltaTime);

        // do a Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGroundLimit to check the ray hits a Collider
        // if returns false, gameObject is further than distToGroundLimit and should be destroyed
        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGroundLimit))
        {
            Destroy(gameObject);
            Debug.Log("Ninja too high, destroyed");
        }
    }

    void SetNewTarget(Vector3 new_target)
    {
        target = new_target;
        transform.LookAt(target);
    }
}
