using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class DragonController : MonoBehaviour
{
    public float health;
    public float damage;
    public float speed;

    public HealthBar healthBar;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public NavMeshAgent agent;
    public GameObject zone;

    public TextMeshProUGUI textholder;

    public Animator animator;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    bool gettingBackInZone;
    Vector3 randomBackInZone;

    bool isChasing;
    public bool isDead;

    public static  bool isAbleToSpawn;
    public bool startOnce;

    private bool asPetAttack;

    public void Awake()
    {
        startOnce = false;
        isAbleToSpawn = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (health < 0)
        {
            print("Win");
            EndScene.IsWon = true;
            SceneManager.LoadScene("EndScene");
        }

        
        if (isAbleToSpawn)
        {

            if (!startOnce)
            {
                isChasing = false;
                healthBar.SetMaxHealth(health);
                agent = GetComponent<NavMeshAgent>();
                gettingBackInZone = false;
                isDead = false;
                player = GameObject.FindGameObjectWithTag("Player").transform;
                animator.SetBool("isWalking", true);
                startOnce = true;
            }

            gameObject.SetActive(true);
            healthBar.SetHealth(this.health);
            EnemyFunctions();

            
        }
    }
   
    private void ReduceHealth(float damage)
    {
        health -= damage;
    }

    private bool validatePlayerIsOutReachable(bool isInSight)
    {
        bool isInArea = zone.GetComponent<Collider>().bounds.Contains(player.position);

        return isInSight && isInArea;
    }

    private void EnemyFunctions()
    {
        playerInSightRange = validatePlayerIsOutReachable(Physics.CheckSphere(transform.position, sightRange, whatIsPlayer));
        playerInAttackRange = validatePlayerIsOutReachable(Physics.CheckSphere(transform.position, attackRange, whatIsPlayer));

        if (IsInsideZone())
        {
            //print("Inside zone");
            if (agent.velocity.magnitude < 0.1f)
            {
                //print("nyangkut");
                //animator.SetBool("isWalking", false);
                GetBackInZone();
            }

            //print("Is inside zone");
            if (!playerInSightRange && !playerInAttackRange)
            {
                //print("diam");
                isChasing = false; 
                animator.SetBool("isChasing", isChasing);
                Patroling();
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                //print("cari");
                isChasing = true;
                animator.SetBool("isChasing", isChasing);
                ChasePlayer();
            }

            if (playerInAttackRange && playerInSightRange)
            {
                //print("attacking");
                AttackPlayer();
            }
        }
        else
        {
            //print("Not in zone");
            GetBackInZone();
        }
    }

    private void Patroling()
    {
        if (animator.GetBool("isWalking") == false) animator.SetBool("isWalking", true);

        //print("Patrol");
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            Collider zoneCollider = zone.GetComponent<Collider>();
           
            walkPoint.x = Mathf.Clamp(walkPoint.x, zoneCollider.bounds.min.x, zoneCollider.bounds.max.x);
            walkPoint.z = Mathf.Clamp(walkPoint.z, zoneCollider.bounds.min.z, zoneCollider.bounds.max.z);

            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

      
        if (distanceToWalkPoint.magnitude < 50f)
            walkPointSet = false;
    }



    private void SearchWalkPoint()
    {
        float randomZ;
        float randomX;
        float finalZ;
        float finalX;

        Collider zoneCollider = zone.GetComponent<Collider>();
        Vector3 minBounds = zoneCollider.bounds.min;
        Vector3 maxBounds = zoneCollider.bounds.max;

        NavMeshHit hit;

        do
        {
            randomZ = Random.Range(-walkPointRange, walkPointRange);
            randomX = Random.Range(-walkPointRange, walkPointRange);

            finalZ = transform.position.z + randomZ;
            finalX = transform.position.x + randomX;

            float y = Terrain.activeTerrain.SampleHeight(new Vector3(finalX, 0, finalZ));

            walkPoint = new Vector3(finalX, y, finalZ);
        }
        while (!NavMesh.SamplePosition(walkPoint, out hit, 1f, NavMesh.AllAreas));

        walkPointSet = true;
    }


    private void ChasePlayer()
    {
       
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //print("Attacking");
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            animator.SetTrigger("attackTrigger");
            ///Attack code here
            ///
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            
        }
    }
    private void ResetAttack()
    {

        //print(PlayerAttributes.GetInstance().Health);
        alreadyAttacked = false; 
        PlayerAttributes.GetInstance().Health -= damage;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
     

        if (health <= 0)
        {
            isDead = true;
            animator.SetTrigger("deadTrigger");
            DestroyEnemy();
            PlayerAttributes.GetInstance().hasWon = true;
        }
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
    private bool IsInsideZone()
    {
        return zone.GetComponent<Collider>().bounds.Contains(transform.position);
    }

    private void GetBackInZone()
    {

        Vector3 distanceToWalkPoint = transform.position - randomBackInZone;

        if (!IsInsideZone())
        {
            if (!gettingBackInZone)
            {
                gettingBackInZone = true;
                randomBackInZone = GetRandomPositionInZone();
                agent.SetDestination(randomBackInZone);
            }
        }

        else if (gettingBackInZone && distanceToWalkPoint.magnitude < 0.5f)
        {
            gettingBackInZone = false;
        }
    }

    private Vector3 GetRandomPositionInZone()
    {

        Collider zoneCollider = zone.GetComponent<Collider>();
        Vector3 minBounds = zoneCollider.bounds.min;
        Vector3 maxBounds = zoneCollider.bounds.max;
        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomZ = Random.Range(minBounds.z, maxBounds.z);

        return new Vector3(randomX, Terrain.activeTerrain.SampleHeight(new Vector3(randomX, 0, randomZ)), randomZ);
    }



}
