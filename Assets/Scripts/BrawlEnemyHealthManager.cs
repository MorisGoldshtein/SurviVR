using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BrawlEnemyHealthManager : MonoBehaviour
{
    public float maxHealthPoints;
    float speed;
    [HideInInspector] // want healthPoints to be accessed by EnemyNavMesh but dont want it visible in Inspector
    public float healthPoints;
    float trap_damage = 20f;
    string current_object;
    float playerDamage = 30f;

    // used to keep track of current score
    int score;

    // If can_add_score is true, then the score can be increased. This is to prevent Update() from increasing the score several times
    // for one death.
    bool can_add_score;
    
    // get score component stored in OVRPlayerController
    Text score_display;

    float distToGroundLimit = 40f;

    // used to respawn an enemy when it's destroyed
    Vector3 enemy_spawn_position;
    
    // use to find the distance from player to ground to check if player is currently grounded (so enemy can't be juggled in air)
    float distToGround;

    Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        // distance from the player pivot to the bottom of the player box collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
        m_Rigidbody = GetComponent<Rigidbody>();
        current_object = gameObject.name;
        enemy_spawn_position = gameObject.transform.position;

        score_display = GameObject.FindWithTag("score").GetComponent<Text>();
        can_add_score = true;
    }

    bool IsGrounded()
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 1.3f);
    }

    bool IsObjGrounded(GameObject gameObject)
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        //return Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(-Vector3.up), distToGround + 1.3f);

        bool groundedDown = Physics.Raycast(gameObject.transform.position, transform.TransformDirection(-Vector3.up), gameObject.GetComponent<Collider>().bounds.extents.y + .01f);
        if(groundedDown){
            Debug.Log("groundedDown");
            return true;
        }
        bool groundedUp = Physics.Raycast(gameObject.transform.position,transform.TransformDirection(-Vector3.up) , gameObject.GetComponent<Collider>().bounds.extents.y + .01f);
        if(groundedUp){
            Debug.Log("groundedUp");
            return true;
        }
        bool groundedLeft = Physics.Raycast(gameObject.transform.position, transform.TransformDirection(-Vector3.up), gameObject.GetComponent<Collider>().bounds.extents.y + .01f);
        if(groundedLeft){
            Debug.Log("groundedLeft");
            return true;
        }
        bool groundedRight = Physics.Raycast(gameObject.transform.position, transform.TransformDirection(-Vector3.up), gameObject.GetComponent<Collider>().bounds.extents.y + .01f);
        if(groundedRight){
            Debug.Log("groundeRight");
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    { 
        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGroundLimit) && can_add_score == true)
        {
            //Destroy(gameObject);
            Debug.Log("Ninja too high, destroyed");
            //score += 100;
            can_add_score = false;
            Invoke(nameof(Respawn), 4f);          
        }

        //score_display.text = "" + score;
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        Debug.Log("Hit registered, " + current_object + " HealthPoints at: " + healthPoints);

        // if damage would set healthPoints to do, gameObject is launched into stratosphere
        if (healthPoints <= 0)
        {
            speed = 5;
            //Destroy(gameObject);
            score += 100;
            Invoke(nameof(Respawn), 4f);
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
        if(!IsObjGrounded(other.gameObject)){
            Debug.Log(other.gameObject.name);
            if(other.gameObject.name.Contains("SpearD") || other.gameObject.name.Contains("Blade"))
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -10f + transform.up * 5f;
                // Fetch the RigidBody component attached to the Ninja GameObject
                //Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
                // Set upward velocity of Ninja Gameobject
                m_Rigidbody.velocity = launchUpward * speed;
                //m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);

                // spear hit is taking off 2x health each time trap is triggered; maybe because spears are hitting twice in quick succession?
                TakeDamage(trap_damage);
            }
            // else if (other.gameObject.name.Contains("CustomHand"))
            // {
            //     Vector3 launchUpward = transform.forward * -10f + transform.up * 5f;
            //     m_Rigidbody.velocity = launchUpward * speed;
            //     TakeDamage(playerDamage);
            // }
            else if(other.gameObject.name.Contains("trash") && !other.gameObject.GetComponent<OVRGrabbable>().isGrabbed && healthPoints > 0 && other.gameObject.GetComponent<OVRGrabbable>().timeSinceLetGo.IsRunning)
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                //Destroy(gameObject);
                score_display.text = (Int32.Parse(score_display.text) + 100).ToString();
                healthPoints = 0;
                Invoke(nameof(Respawn), 3f);
            }
            else if(other.gameObject.name.Contains("chair") && !other.gameObject.GetComponent<OVRGrabbable>().isGrabbed && healthPoints > 0 && other.gameObject.GetComponent<OVRGrabbable>().timeSinceLetGo.IsRunning)
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                //Destroy(gameObject);
                score_display.text = (Int32.Parse(score_display.text) + 100).ToString();
                healthPoints = 0;
                Invoke(nameof(Respawn), 3f);
            }
            else if(other.gameObject.name.Contains("table") && !other.gameObject.GetComponent<OVRGrabbable>().isGrabbed && healthPoints > 0 && other.gameObject.GetComponent<OVRGrabbable>().timeSinceLetGo.IsRunning)
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                //Destroy(gameObject);
                score_display.text = (Int32.Parse(score_display.text) + 100).ToString();
                healthPoints = 0;
                Invoke(nameof(Respawn), 3f);
            }
            else if(other.gameObject.name.Contains("cash") && !other.gameObject.GetComponent<OVRGrabbable>().isGrabbed && healthPoints > 0 && other.gameObject.GetComponent<OVRGrabbable>().timeSinceLetGo.IsRunning)
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                //Destroy(gameObject);
                score_display.text = (Int32.Parse(score_display.text) + 100).ToString();
                healthPoints = 0;
                Invoke(nameof(Respawn), 3f);
            }
            else if(other.gameObject.name.Contains("flowers") && !other.gameObject.GetComponent<OVRGrabbable>().isGrabbed && healthPoints > 0 && other.gameObject.GetComponent<OVRGrabbable>().timeSinceLetGo.IsRunning)
            {
                Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed something");
                //Destroy(gameObject);
                score_display.text = (Int32.Parse(score_display.text) + 100).ToString();
                healthPoints = 0;
                Invoke(nameof(Respawn), 3f);
            }
        }
    
    }

    private void Respawn()
    {
        gameObject.transform.position = enemy_spawn_position;
        healthPoints = maxHealthPoints;
        Debug.Log("Respawned with " + healthPoints + " health");
    }
}
