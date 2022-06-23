using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.VFX;
using System.Threading;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //imported scripts
    public PlayerAI player1;
    public SpawnManager sM;


    public TMP_Text monsterName;
    public TMP_Text damageText;

    public TMP_Text HPc;
    public TMP_Text HPt;
    
    //enemy stat variables
    public string mName;
    public string damageTaken;
    public float maxHealth = 10f;
    public float currentHealth = 10f;
    public float timer = 1.0f;
    public float elapsedTime = 0.01f;

    public float alpha = 1.0f;

    public VisualEffect burst;

    //Walkpoint variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //enemy attack variables
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public bool alreadyAttacked;
    public float timeBetweenAttacks = 1.0f;

    public float damage = 1.0f;

    private void Awake()
    {
        sM = GameObject.Find("GameScriptMaster").GetComponent<SpawnManager>();
        player = GameObject.Find("PlayerObj").transform;
        player1 = GameObject.Find("PlayerObj").GetComponent<PlayerAI>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        NameMonster();
        currentHealth = maxHealth;
        HPt.SetText(maxHealth.ToString());
    }
    private void Update()
    {
        alpha = Mathf.Lerp(1,0, elapsedTime/timer);


        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }
    private void DamageDisplay(float damage)
    {
      damageTaken = damage.ToString();
      damageText.SetText("-"+damageTaken + " HP");
      elapsedTime = 0.01f;
    }
    private void DamageDisplay()
    {
       elapsedTime += Time.deltaTime;
       damageText.alpha = alpha;
    }
    private void FixedUpdate()
    {
       DamageDisplay();
       HPc.SetText(currentHealth.ToString() + " /");
       HPt.SetText(maxHealth.ToString() + " HP");

       if (!playerInSightRange && !playerInAttackRange) Patroling();
       if (playerInSightRange && !playerInAttackRange) ChasePlayer();
       if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }
    private void NameMonster()
    {
        monsterName.SetText(mName);
    }
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            //Instantiate(burst, player.position, Quaternion.identity);
            player1.TakeDamage(damage);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void KnockBack(Vector3 other, float knockbackDistance)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 moveDirection = rb.transform.position - other;
        Vector3 knockback = moveDirection.normalized * knockbackDistance;
        rb.AddForce(knockback * knockbackDistance);
        rb.AddForce(rb.transform.up * (knockbackDistance));
        Debug.Log("Knocked back " + knockback.ToString());
        Stun();
    }
    private void Stun()
    {
        float stun = 1000;
        stun -= Time.deltaTime;
        if (stun > 0)
        {
            agent.speed = 0.0f;
        }
        if (stun <= 0)
        {
            agent.speed = 8.0f;
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        DamageDisplay(damage);
        if (currentHealth <= 0 && gameObject != null)
        {
            this.GetComponent<Rigidbody>().detectCollisions = false;
            sM.IDied();
            Destroy(gameObject);
        }
    }
}
