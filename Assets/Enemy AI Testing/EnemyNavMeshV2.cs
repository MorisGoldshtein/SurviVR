using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshV2 : MonoBehaviour
{
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

    //  States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        alreadyAttacked = false;
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }


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

    }

    private void Patroling()
    {
        if (!walkPointSet) 
            SearchWalkPoint();

        if (walkPointSet)
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
        navMeshAgent.destination = movePositionTransform.position; 
    }

    private void AttackPlayer()
    {
        //  Make sure enemy doesn't move
        navMeshAgent.SetDestination(transform.position);

        transform.LookAt(movePositionTransform);

        if (!alreadyAttacked) 
        {
            /// Attack code here!
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            /// 

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() 
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
