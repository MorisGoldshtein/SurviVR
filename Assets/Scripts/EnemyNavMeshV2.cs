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

    private EnemyHealthManager health_script;

    //  Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //  Chasing

    //  Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;
    bool canWalk;   // set to false for 2 seconds after an attack so enemy can't move while punching
    bool isAlive; // set to True if enemy's health is >= 0

    // distance from the player pivot to the bottom of the player box collider
    float distToGround;

    Rigidbody enemy_Rigidbody;
    GameObject hand_gameObject; // for detecting enemy punches
    GameObject leg_gameObject;  // for detecting enemy kicks

    // various colliders meant to be turned on and off for death animation
    Collider ninja_collider;
    Collider spine_collider;
    Collider left_leg_collider;


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
        leg_gameObject = transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
        // get the "hiyah" audio file attached to the AudioSource component of the enemy
        m_punch = GetComponent<AudioSource>();
        //set to false or else sound effect will play when scene is first starting up
        m_punch.enabled = false;
        canWalk = true;
        isAlive = true;
        health_script = GetComponent<EnemyHealthManager>();

        ninja_collider = GetComponent<Collider>();
        ninja_collider.enabled = true;

        spine_collider = transform.GetChild(1).GetChild(2).GetComponent<Collider>();
        spine_collider.enabled = false;

        left_leg_collider = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Collider>();
        left_leg_collider.enabled = false;
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

        if (!playerInSightRange && !playerInAttackRange && isAlive) Patroling();
        if (playerInSightRange && !playerInAttackRange && isAlive) ChasePlayer();
        if (playerInSightRange && playerInAttackRange && isAlive) AttackPlayer();
        
        if (health_script.healthPoints <= 0 && isAlive)
        {
            ninja_collider.enabled = false;
            spine_collider.enabled = true;
            left_leg_collider.enabled = true;
            navMeshAgent.enabled = false;
            isAlive = false;
            canWalk = false;
            m_Animator.Rebind(); // resets enemy animations
            m_Animator.SetTrigger("DeathTrigger"); // starts death animation
            Invoke(nameof(ResetAfterDeath), 10f);
        }

        // if (health_script.healthPoints > 0 && !isAlive)
        // {
        //     ResetAfterDeath();
        // }

        if (!Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), 10f))
        {
            //Debug.Log("Fall through trapdoor");
            navMeshAgent.enabled = false;
        }

        if(Mathf.Abs(enemy_Rigidbody.velocity.y) <= 0.01 && navMeshAgent.enabled == false && canWalk && isAlive)
        {
            Debug.Log("NavMeshAgent re-enabled");
            navMeshAgent.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("y-velocity: " + enemy_Rigidbody.velocity.y);
            Debug.Log(" IsGrounded: " + IsGrounded());
            Debug.Log(" navMeshEnabled: " + navMeshAgent.enabled);
            Debug.Log(" canWalk: " + canWalk);
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
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            navMeshAgent.enabled = false;
            canWalk = false;

            int random_attack = Random.Range(1,10); // random number from 1 to 10

            if (random_attack > 3) // 70% chance to throw a punch
            {
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
            else    // 30% chance to throw a kick
            {
                m_punch.enabled = true; // enables audio component
                m_punch.Play(); // plays audio "hiyah" clip
                m_Animator.Rebind(); // resets enemy animations
                Invoke(nameof(KickTrigger), 0.5f); // starts kick animation; need to delay start of animation to sync up sound
                Rigidbody leg_Rigidbody = leg_gameObject.GetComponent<Rigidbody>();
                leg_Rigidbody.AddForce(transform.forward * 15f, ForceMode.Impulse);
                leg_Rigidbody.AddForce(transform.up * 8f, ForceMode.Impulse);
                alreadyAttacked = true;
                Invoke(nameof(AllowKickCollision), 0.75f);
                Invoke(nameof(ResetAfterAttack), 1.75f);
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
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

    private void KickTrigger() // used to disable hand collider and allow enemy to walk again after 1.5 seconds (i.e. length of time it takes to finish 1 punch animation)
    {
        m_Animator.SetTrigger("KickTrigger");
    }


    private void AllowPunchCollision() // used to disable hand collider and allow enemy to walk again after 1.5 seconds (i.e. length of time it takes to finish 1 punch animation)
    {
        Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
        hand_Collider.enabled = true;
    }

    private void AllowKickCollision() // used to disable hand collider and allow enemy to walk again after 1.5 seconds (i.e. length of time it takes to finish 1 punch animation)
    {
        Collider leg_Collider = leg_gameObject.GetComponent<Collider>();
        leg_Collider.enabled = true;
    }

    private void ResetAfterAttack() // used to disable hand collider and allow enemy to walk again after attack animation is finished
    {
        Collider hand_Collider = hand_gameObject.GetComponent<Collider>();
        hand_Collider.enabled = false;
        Collider leg_Collider = leg_gameObject.GetComponent<Collider>();
        leg_Collider.enabled = false;
        m_punch.enabled = false;
        canWalk = true;
    }
    
    private void ResetAfterDeath() // used to reset parameters after death animation has played
    {
        m_Animator.Rebind(); // resets enemy animations
        ninja_collider.enabled = true;
        spine_collider.enabled = false;
        left_leg_collider.enabled = false;
        canWalk = true;
        isAlive = true;
        navMeshAgent.enabled = true;
    }

    private void ResetAttack() // used to allow enemy to attack again
    {
        alreadyAttacked = false;
    }

    // private void TakeDamage(int damage) 
    // {
    //     health -= damage;

    //     if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    // }

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
