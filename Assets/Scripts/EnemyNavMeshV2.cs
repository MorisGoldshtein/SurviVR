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
    }


    // Update is called once per frame
    void Update()
    {
        //  Go to marked location
        // navMeshAgent.destination = movePositionTransform.position; 

        //  Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // playerInSightRange = false;  //  for testing purposes
        // playerInAttackRange = false; //  for testing purposes

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        
        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), 10f))
        {
            //Debug.Log("Fall through trapdoor");
            navMeshAgent.enabled = false;
        }

        if(enemy_Rigidbody.velocity.y == 0 && navMeshAgent.enabled == false)
        {
            //Debug.Log("NavMeshAgent re-enabled");
            navMeshAgent.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("y-velocity: " + enemy_Rigidbody.velocity.y);
            Debug.Log(" IsGrounded: " + IsGrounded());
            Debug.Log(" navMeshEnabled: " + navMeshAgent.enabled);
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
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        
            m_punch.enabled = true;
            m_punch.Play();
            m_Animator.SetTrigger("PunchTrigger");
            Rigidbody hand_Rigidbody = hand_gameObject.GetComponent<Rigidbody>();
            Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
            hand_Collider.enabled = true;
            hand_Rigidbody.AddForce(transform.forward * 15f, ForceMode.Impulse);
            hand_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    bool IsGrounded()
    {
        // enemy is grounded if it's not falling
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), distToGround + 1.3f);
    }

    //Code triggers on collision with another GameObject that has a Collider component with Is Trigger box checked
    private void OnTriggerEnter(Collider other)
    {
        // Currently meant for collision with SpearD objects inside SpikeTrapD objects
        if (IsGrounded() && other.gameObject.name.Contains("SpearD")) // if the GameObject is currently airborne, it shouldn't be allowed to be launched again
        {
            //Debug.Log("navMeshAgent disabled");
            navMeshAgent.enabled = false;
        }
        
    }


    private void ResetAttack() 
    {
        alreadyAttacked = false;
        Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
        hand_Collider.enabled = false;
        //m_punch.enabled = false;
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
