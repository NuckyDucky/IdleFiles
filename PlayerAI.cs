using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class PlayerAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject enemy;
    public LayerMask whatIsGround, whatIsEnemy;

    //Attacking
    public float attackTimer = 2.0f;
    public GameObject attackPoint;
    public float timeBetweenAttacks;
    public float projectileSpeed = 32.0f;
    public float distance;

    public bool alreadyAttacked;

    public VisualEffect dragonFire;

    //spells and effects
    public GameObject attackObject;
    public GameObject attackObject1;
    public GameObject attackObject2;

    //Walkpoint variables
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float fleeDistance = 10f;

    public float sightRange, attackRange;
    public bool enemyInSightRange, enemyInAttackRange;

    //combat variables
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public float damage = 1.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate()
    {
        FindClosestEnemy();
        if (enemy != null){
        distance = Vector3.Distance(transform.position, enemy.transform.position);
        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
        }
        {
        if (!enemyInSightRange && !enemyInAttackRange && enemy != null) Patroling();
        if (enemyInSightRange && enemyInAttackRange && enemy != null) AttackEnemy();
        if (distance < fleeDistance && alreadyAttacked && enemy != null) RunFromEnemy();
        }
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
    private void RunFromEnemy()
    {
        Vector3 dirToPlayer = transform.position - enemy.transform.position;

        Vector3 newPos = transform.position + dirToPlayer;

        agent.SetDestination(newPos);
    }
    private void ChaseEnemy()
    {
        agent.SetDestination(enemy.transform.position);
    }
    private void AttackEnemy()
    {
        GameObject _attackObject = attackObject;
        agent.SetDestination(transform.position);

        //transform.LookAt(enemy.transform);
        transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.LookRotation(enemy.transform.position - transform.position ), 0.3f);

        if (!alreadyAttacked)
        {
            float chooseSkill = Random.Range(1.0f, 100.0f);
            if (chooseSkill < 75.0f) _attackObject = attackObject;
            if (chooseSkill < 45.0f) _attackObject = attackObject1;
            if (chooseSkill < 25.0f) _attackObject = attackObject2;

            Rigidbody rb = Instantiate(_attackObject, attackPoint.transform.position + Vector3.up, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * (projectileSpeed * 1 + Time.deltaTime), ForceMode.Impulse);
           // rb.AddForce(transform.up * (projectileSpeed/8), ForceMode.Impulse);
           // dragonFire.SendEvent("PlayDragonFire");
            Debug.Log("dragonfire!");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) GameOver();
    }
    void GameOver()
    {
        Debug.Log("Game over!");
    }
	void FindClosestEnemy()
	{
		float distanceToClosestEnemy = Mathf.Infinity;
		EnemyAI closestEnemy = null;
		EnemyAI[] allEnemies = GameObject.FindObjectsOfType<EnemyAI>();

		foreach (EnemyAI currentEnemy in allEnemies) {
			float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
			if (distanceToEnemy < distanceToClosestEnemy) {
				distanceToClosestEnemy = distanceToEnemy;
				closestEnemy = currentEnemy;
                enemy = closestEnemy.gameObject;
			}
		}
	}
}
