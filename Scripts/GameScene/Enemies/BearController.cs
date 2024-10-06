
using UnityEngine;
using UnityEngine.AI;

public class BearController : MonoBehaviour
{
    public HealthBar healthBar;
    public GameObject zone;
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Animation
    public Animator animator;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    bool gettingBackInZone;
    Vector3 randomBackInZone;

    public bool becomePet;

    GameObject isAttackingThisObject;

    private void Awake()
    {
        health = 100;
        healthBar.SetMaxHealth(health);
        agent = GetComponent<NavMeshAgent>();
        gettingBackInZone = false;
        becomePet = false;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.SetBool("isWalking", true);
    }

    private void Update()
    {
        healthBar.SetHealth(this.health);

        if (!becomePet)
        {
            EnemyFunctions();
        }
        else
        {
            PetFunctions();
        }
    }

    private void PetFunctions()
    {

        GameObject playerIsAttacking = player.gameObject.GetComponent<PlayerMovement>().IsAttackingWhat;
        isAttackingThisObject = playerIsAttacking;

        if (playerIsAttacking != null && playerIsAttacking != this) {
            agent.SetDestination(playerIsAttacking.transform.position);

            transform.LookAt(playerIsAttacking.transform.position);

            if (!alreadyAttacked)
            {
                animator.SetTrigger("attackTrigger");

                alreadyAttacked = true;
                Invoke(nameof(ResetAttackPet), timeBetweenAttacks);
                
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    private void ResetAttackPet()
    {
        isAttackingThisObject.GetComponent<BearController>().ReduceHealth(10);
        alreadyAttacked = false;
    }

    private void ReduceHealth(float damage)
    {
        health -= damage;
    }

    private void EnemyFunctions()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (IsInsideZone())
        {
            if (agent.velocity.magnitude < 0.1f)
            {
                print("nyangkut");
                animator.SetBool("isWalking", false);
                GetBackInZone();
            }

            //print("Is inside zone");
            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();
                //print("diam");
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                print("chasing");
                ChasePlayer();
            }

            if (playerInAttackRange && playerInSightRange)
            {
                print("attacking");
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
            // Constrain the walkPoint within the zone's bounds
            walkPoint.x = Mathf.Clamp(walkPoint.x, zoneCollider.bounds.min.x, zoneCollider.bounds.max.x);
            walkPoint.z = Mathf.Clamp(walkPoint.z, zoneCollider.bounds.min.z, zoneCollider.bounds.max.z);

            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
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
        //print("Chasing");
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
        PlayerAttributes.GetInstance().Health -= 5;
        print(PlayerAttributes.GetInstance().Health);   
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("IsHit");

        if (health <= 0)
        {
            becomePet = true;
            health = 100;
            PlayerAttributes.GetInstance().Pets.Add(gameObject);
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
        // Check if the enemy is inside the trigger zone
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

        return new Vector3(randomX, Terrain.activeTerrain.SampleHeight(new Vector3(randomX,0,randomZ)), randomZ);
    }
}
