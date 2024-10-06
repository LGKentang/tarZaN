using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour
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

    public bool becomePet;
    GameObject isAttackingThisObject;

    private bool asPetAttack;

    private bool targeting;

    private void Awake()
    {
        health = 100;
        healthBar.SetMaxHealth(health);
        agent = GetComponent<NavMeshAgent>();
        gettingBackInZone = false;
        becomePet = false;
        asPetAttack = false;
        targeting = false;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        GameObject bearObj = null;

        if (TDManager.npcs.Count > 0)
        {
           
            targeting = true;
            float max = Mathf.Infinity;
            GameObject targetedObject = null;
            foreach (GameObject go in TDManager.npcs)
            {
                float distance = (go.transform.position - transform.position).magnitude; 
                if (distance < max)
                {
                    targetedObject = go;    
                    max = distance;
                }
            }

            //if (((targetedObject.transform.position - transform.position).magnitude > 1f))
            //{
                agent.SetDestination(targetedObject.transform.position);

            //}

        }
        else
        {
            targeting = false;
        }



        foreach (Collider collider in colliders)
        {
            bearObj = collider.gameObject;
            if ((collider.CompareTag("Bear") || collider.CompareTag("TDEnemy") )&& !PlayerAttributes.GetInstance().IsPet(bearObj))
            {
                //print("found");

                transform.LookAt(bearObj.transform);

                if (!asPetAttack && ((bearObj.transform.position - transform.position).magnitude) > 2f)
                {
                    agent.SetDestination(bearObj.transform.position);
                }

                Collider[] attackColliders = Physics.OverlapSphere(transform.position ,attackRange);
                GameObject attackBearObj = null;

                foreach (Collider attackCollider in attackColliders)
                {
                    if ((attackCollider.CompareTag("Bear")||attackCollider.CompareTag("TDEnemy")) && !PlayerAttributes.GetInstance().IsPet(attackBearObj))
                    {
                        transform.LookAt(bearObj.transform);
                        if (!asPetAttack)
                        {
                            transform.LookAt(bearObj.transform);
                            animator.SetTrigger("attackTrigger");
                            if (bearObj.GetComponent<TDEnemy>() == null)
                            {
                                bearObj.GetComponent<AnimalController>().TakeDamage((int) damage);
                                if (bearObj.GetComponent<AnimalController>().becomePet)
                                {
                                    PlayerAttributes.GetInstance().AddExperience(health);
                                }

                            }
                            else
                            {
                                bearObj.GetComponent<TDEnemy>().TakeDamage((int)damage);
                                if (bearObj.GetComponent<TDEnemy>().IsDead() || bearObj == null)
                                {
                                    PlayerAttributes.GetInstance().AddExperience(health);
                                }
                            }
                           
                            asPetAttack = true;
                            Invoke(nameof(ResetAttackPet), timeBetweenAttacks);
                        }

                    }

                }


            }
        }

        //print(targeting);
       
        if (!asPetAttack && !targeting)
        {
          agent.SetDestination(player.position);
        }
    }

  
    private void ResetAttackPet()
    {
        asPetAttack = false;
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
                Patroling();
                //print("diam");
            }

            if (playerInSightRange && !playerInAttackRange)
            {
                //print("chasing");
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

        // sampe
        if (distanceToWalkPoint.magnitude < 2f)
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

        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            animator.SetTrigger("attackTrigger");
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
        animator.SetTrigger("isHit");

        if (health <= 0)
        {
            //PlayerAttributes.GetInstance().AddExperience(health);
           
            becomePet = true;
            health = 100;
            textholder.text = "Pet";
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
