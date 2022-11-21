using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    float maxHealthPoints = 200f;
    float speed;
    float healthPoints;
    float spearDamage = 20f;
    string current_object;
    
    // use to find the distance from player to ground to check if player is currently grounded (so enemy can't be juggled in air)
    float distToGround;

    Rigidbody m_Rigidbody;

    // This is the AI that is being controlled
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        // distance from the player pivot to the bottom of the player box collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
        m_Rigidbody = GetComponent<Rigidbody>();
        current_object = gameObject.name;
    }

    bool IsGrounded()
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 1.3f);
    }

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.N))
        {
            Vector3 launchUpward = transform.forward * -20f + transform.up * 15f;
            m_Rigidbody.velocity = launchUpward * 1;
        }
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        //Debug.Log("Hit registered, " + current_object + " HealthPoints at: " + healthPoints);

        // if damage would set healthPoints to do, gameObject is launched into stratosphere
        if (healthPoints <= damage)
        {
            speed = 5;
            //Destroy(gameObject);
        }
        else
        {
            // slowly incremement launch speed based on how much damage GameObject has taken to a maximum of 1.5
            speed = Mathf.Min(Mathf.Abs(maxHealthPoints - healthPoints) / 100, 3/2);
        }
    }


    //Code triggers on collision with another GameObject that has a Collider component with Is Trigger box checked
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("IsGrounded: " + IsGrounded());
        //Debug.Log("Name of the colliding object: " + other.gameObject.name);
        //Debug.Log("Name of the collided with object: " + gameObject.name);
        // Currently meant for collision with SpearD objects inside SpikeTrapD objects
        if (IsGrounded()) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        {
            if(other.gameObject.name.Contains("SpearD"))
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -10f + transform.up * 5f;
                // Fetch the RigidBody component attached to the Ninja GameObject
                //Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
                // Set upward velocity of Ninja Gameobject
                m_Rigidbody.velocity = launchUpward * speed;
                //m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);

                // spear hit is taking off 2x health each time trap is triggered; maybe because spears are hitting twice in quick succession?
                TakeDamage(spearDamage);
            }
            else if(other.gameObject.name.Contains("Ocean"))
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                Destroy(gameObject);
            }
        }
    }
}