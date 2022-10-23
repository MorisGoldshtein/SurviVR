using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHealth : MonoBehaviour
{
    float maxHealthPoints = 200f;
    float speed;
    float healthPoints;
    float spearDamage = 20f;
    
    // use to find the distance from player to ground to check if player is currently grounded (so enemy can't be juggled in air)
    float distToGround;

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        // distance from the player pivot to the bottom of the player box collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    bool IsGrounded()
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround);
    }

    // Update is called once per frame
    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        Debug.Log("Hit registered, HealthPoints at: " + healthPoints);

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
        // Currently meant for collision with SpearD objects inside SpikeTrapD objects
        if (IsGrounded()) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        {
            // Create a new Vector for launching GameObject upwards
            Vector3 launchUpward = new Vector3(0.0f, 6.0f, 0.0f);
            // Fetch the RigidBody component attached to the Ninja GameObject
            Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
            // Set upward velocity of Ninja Gameobject
            m_Rigidbody.velocity = launchUpward * speed;

            // spear hit is taking off 2x health each time trap is triggered; maybe because spears are hitting twice in quick succession?
            TakeDamage(spearDamage);
        }
    }
}

