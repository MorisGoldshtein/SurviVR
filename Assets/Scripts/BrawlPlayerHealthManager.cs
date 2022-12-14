using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrawlPlayerHealthManager : MonoBehaviour
{
    Slider healthbar;
    public float maxHealthPoints;
    float speed;
    float healthPoints;
    float spearDamage = 20f;
    string current_object;
    AudioSource m_hit;
    
    // use to find the distance from player to ground to check if player is currently grounded (so enemy can't be juggled in air)
    float distToGround;

    Rigidbody m_Rigidbody;

    CharacterController m_CharCon;

    // This is the AI that is being controlled
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        // distance from the player pivot to the bottom of the player box collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CharCon = GetComponent<CharacterController>();

        current_object = gameObject.name;
        healthbar = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        healthbar.maxValue = maxHealthPoints;

        // get the "air-whistle-punch" audio file attached to the AudioSource component of the HitSound
        // child component of OVRPlayerController
        //m_hit = transform.GetChild(2).GetComponent<AudioSource>();
        m_hit = GameObject.FindWithTag("HitSound").GetComponent<AudioSource>();
        //set to false or else sound effect will play when scene is first starting up
        // m_hit.enabled = false;
    }

    bool IsGrounded()
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(current_object + " IsGrounded: " + IsGrounded());
            m_hit.Play();
            //hand_Rigidbody.AddForce(transform.forward * 15f, ForceMode.Impulse);
            //m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
        }
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        Debug.Log("Hit registered, " + current_object + " HealthPoints at: " + healthPoints);
        healthbar.value = healthPoints;

        // if damage would set healthPoints to do, gameObject is launched into stratosphere
        if (healthPoints <= damage)
        {
            // load DeathScreen scene
            //UnityEngine.SceneManagement.SceneManager.LoadScene("DeathScreen");
            GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene("DeathScreen");
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
        //Debug.Log("Name of the collided with object: " + current_object);
        // Currently meant for collision with SpearD objects inside SpikeTrapD objects
        //if (IsGrounded()) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        //{
            if(other.gameObject.name.Contains("SpearD"))
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -10f + transform.up * 5f;
                // Fetch the RigidBody component attached to the Ninja GameObject
                Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
                // Set upward velocity of Ninja Gameobject
                m_Rigidbody.velocity = launchUpward * speed;
                //m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);

                // spear hit is taking off 2x health each time trap is triggered; maybe because spears are hitting twice in quick succession?
                TakeDamage(spearDamage);
            }
            else if(other.gameObject.name.Contains("mixamorig:RightHand") && current_object == "OVRPlayerController")
            {
                // Create a new Vector for launching GameObject upwards
                //Vector3 launchUpward = new Vector3(-10.0f, 20.0f, 0.0f);
                Vector3 launchUpward = transform.forward * -10f + transform.up * 3f;
                // Set upward velocity of Ninja Gameobject
                //m_Rigidbody.velocity = launchUpward * speed;
                //m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
                m_hit.Play();
                TakeDamage(10);
            }
            else if(other.gameObject.name.Contains("mixamorig:RightLeg") && current_object == "OVRPlayerController")
            {
                // Create a new Vector for launching GameObject upwards
                m_hit.Play();   // play sound of getting hit
                TakeDamage(7);
            }
            else if(other.gameObject.name.Contains("Ocean"))
            {
                //Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed: " + current_object);
                //Destroy(gameObject);

                // load DeathScreen scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("DeathScreen");
            }
        //}
    }
}