using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FistPlayerHealthManager : MonoBehaviour
{
    float maxHealthPoints = 1000f;
    float speed;
    float healthPoints;
    float trap_damage = 5f;
    float punch_damage = 30f;
    float kick_damage = 50f;
    string current_object;
    AudioSource m_hit;
    Slider healthBar;

    public SceneChanger scene_changer;
    
    // use to find the distance from player to ground to check if player is currently grounded (so enemy can't be juggled in air)
    float distToGround;

    Rigidbody m_Rigidbody;

    // This is the AI that is being controlled
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    public int sceneBuildIndex;

    // used for scene switching
    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = maxHealthPoints;
        // distance from the player pivot to the bottom of the player box collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
        m_Rigidbody = GetComponent<Rigidbody>();
        current_object = gameObject.name;

        // get the Slider component corresponding to the player's health bar and set to maxHealth
        healthBar = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        healthBar.maxValue = maxHealthPoints;
        healthBar.value = maxHealthPoints;

        // get the "air-whistle-punch" audio file attached to the AudioSource component of the HitSound
        // child component of OVRPlayerController
        m_hit = transform.GetChild(2).GetComponent<AudioSource>();
        //set to false or else sound effect will play when scene is first starting up
        m_hit.enabled = false;
    }

    bool IsGrounded()
    {
        // do a short Raycast starting from transform.position in the -Vector3.up direction (i.e. downwards) a distance of distToGround to check the ray hits a Collider
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // keyboard input used for testing purposes only; not available in VR mode
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(current_object + " IsGrounded: " + IsGrounded());
            m_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
        }
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        healthBar.value = healthPoints;

        //Debug.Log("Hit registered, " + current_object + " HealthPoints at: " + healthPoints);

        if (healthPoints <= damage)
        {
            // load DeathScreen scene
            Destroy(gameObject);
            GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene(NameFromIndex(6));
        }
        else
        {
            // slowly incremement launch speed based on how much damage GameObject has taken to a maximum of 0.5
            speed = Mathf.Min(Mathf.Abs(maxHealthPoints - healthPoints) / 100, 0.5f);
        }
    }

    //Code triggers on collision with another GameObject that has a Collider component with Is Trigger box checked
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Name of the colliding object: " + other.gameObject.name);
        //Debug.Log("Name of the collided with object: " + current_object);
        
        if (IsGrounded()) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        {
            if(other.gameObject.name.Contains("SpearD") || other.gameObject.name.Contains("Blade"))
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -10f + transform.up * 5f;
                // Fetch the RigidBody component attached to the Ninja GameObject
                Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
                // Set upward velocity of Ninja Gameobject
                m_Rigidbody.velocity = launchUpward * speed;

                // spear hit is taking off 2x health each time trap is triggered; maybe because spears are hitting twice in quick succession?
                TakeDamage(trap_damage);
            }
            else if(other.gameObject.name.Contains("mixamorig:RightLeg") && current_object == "OVRPlayerController")
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -7f + transform.up * 2f;
                m_Rigidbody.velocity = launchUpward * speed;
                m_hit.Play();   // play sound of getting hit
                TakeDamage(kick_damage);
            }
            else if(other.gameObject.name.Contains("mixamorig:RightHand") && current_object == "OVRPlayerController")
            {
                // Create a new Vector for launching GameObject upwards
                Vector3 launchUpward = transform.forward * -7f + transform.up * 2f;
                m_Rigidbody.velocity = launchUpward * speed;
                m_hit.Play();   // play sound of getting hit
                TakeDamage(punch_damage);
            }
            else if(other.gameObject.name.Contains("Ocean"))
            {
                //Debug.Log("Name of the object: " + other.gameObject.name);
                Debug.Log("Destroyed: " + current_object);
                Destroy(gameObject);

                // load DeathScreen scene
                GameObject.FindWithTag("SceneThing").GetComponent<SceneChanger>().LoadScene(NameFromIndex(6));
            }
        }
    }
}