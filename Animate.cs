using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class Animate : MonoBehaviour
{
    private Animator anim;
    private EnemyAI enemy;
    private NavMeshAgent nAgent;

    public float velocity;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
        anim = GetComponentInChildren<Animator>();
        nAgent = GetComponent<NavMeshAgent>();

    }
    private void FixedUpdate()
    {
        velocity = nAgent.velocity.magnitude;
        if (velocity > 0.01f)
        {
            anim.SetFloat("Speed",(1 + (velocity * 1+Time.deltaTime)));
        }
    }
}
