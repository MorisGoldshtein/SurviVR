using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshV2 : MonoBehaviour
{
    Animator m_Animator;
    // for the "hiyah" sound
    AudioSource m_punch;

    [SerializeField] private Transform movePositionTransform;   //  This is the target you are going to
    private NavMeshAgent navMeshAgent;  //  This is the AI that is being controlled

    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    //  Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //  Chasing

    //  Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;
    bool canWalk;   // set to false for few seconds after an attack so enemy can't move while attacking

    // distance from the player pivot to the bottom of the player box collider
    float distToGround;

    Rigidbody enemy_Rigidbody;
    GameObject hand_gameObject;

    //  States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        alreadyAttacked = false;
    }

    // // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        enemy_Rigidbody = GetComponent<Rigidbody>();
        hand_gameObject = transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;

        // get the "hiyah" audio file attached to the AudioSource component of the enemy
        m_punch = GetComponent<AudioSource>();
        //set to false or else sound effect will play when scene is first starting up
        m_punch.enabled = false;
        canWalk = true;
    }


    // Update is called once per frame
    void Update()
    {
        //  Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        
        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), 10f))
        {
            //Debug.Log("Fall through trapdoor");
            navMeshAgent.enabled = false;
        }

        if(enemy_Rigidbody.velocity.y == 0 && navMeshAgent.enabled == false && canWalk)
        {
            //Debug.Log("NavMeshAgent re-enabled");
            navMeshAgent.enabled = true;
        }

        // keyboard input used for testing purposes only; not available in VR mode
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("y-velocity: " + enemy_Rigidbody.velocity.y);
            Debug.Log(" IsGrounded: " + IsGrounded());
            Debug.Log(" navMeshEnabled: " + navMeshAgent.enabled);
        }

        // test keybind for launching enemy backwards
        if (Input.GetKeyDown(KeyCode.N))
        {
            Rigidbody m_Rigidbody = GetComponent<Rigidbody>();
            navMeshAgent.enabled = false;
            Vector3 launchUpward = transform.forward * -20f + transform.up * 15f;
            m_Rigidbody.velocity = launchUpward * 1;
        }

    }

    private void Patroling()
    {
        if (!walkPointSet) 
            SearchWalkPoint();

        if (walkPointSet && navMeshAgent.enabled)
            navMeshAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //  WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint() 
    {
          //  Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        //  Go to marked location
        if (navMeshAgent.enabled)
            navMeshAgent.destination = movePositionTransform.position; 
    }

    private void AttackPlayer()
    {
        //  Make sure enemy doesn't move
        if (navMeshAgent.enabled)
            navMeshAgent.SetDestination(transform.position);

        //transform.LookAt(movePositionTransform);

        if (!alreadyAttacked)
        {
            /// Attack code here!
            navMeshAgent.enabled = false;
            canWalk = false;
            m_punch.enabled = true; // enables audio component
            m_punch.Play(); // plays audio "hiyah" clip
            m_Animator.Rebind(); // resets enemy animations
            m_Animator.SetTrigger("PunchTrigger"); // starts punch animation
            Rigidbody hand_Rigidbody = hand_gameObject.GetComponent<Rigidbody>();
            hand_Rigidbody.AddForce(transform.forward * 15f, ForceMode.Impulse);
            hand_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(AllowPunchCollision), 1.0f);
            Invoke(nameof(ResetAfterAttack), 1.5f);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    bool IsGrounded()
    {
        // check if enemy is currently standing on ground or close to it
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 1.3f);
    }

    //Code triggers on collision with another GameObject that has a Collider component with Is Trigger box checked
    private void OnTriggerEnter(Collider other)
    {
        if (IsGrounded() && other.gameObject.name.Contains("SpearD")) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        {
            //Debug.Log("navMeshAgent disabled");
            navMeshAgent.enabled = false;
        }
    }

    private void AllowPunchCollision() // used to enable hand collider and allow enemy punch to land
    {
        Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
        hand_Collider.enabled = true;
    }

    private void ResetAfterAttack() // used to disable hand collider and allow enemy to walk again after attack animation is finished
    {
        Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
        hand_Collider.enabled = false;
        m_punch.enabled = false;
        canWalk = true;
    }

    private void ResetAttack() // used to allow enemy to attack again
    {
        alreadyAttacked = false;
    }

    private void TakeDamage(int damage) 
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}