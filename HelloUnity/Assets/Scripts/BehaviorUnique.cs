using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI;

public class BehaviorUnique : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField]
    private float agentAcceleration = 6.0f;
    [SerializeField]
    private float agentSpeed = 12.0f;
    [SerializeField]
    private float attackRange = 3.0f; 
    [SerializeField]
    private float walkAnimThreshold = 0.2f;
    [SerializeField]
    private float followDistance = 5.0f;

    [Header("environment")]
    [SerializeField]
    private Transform player; 

    private Transform nearestEnemy;

    // 0 - idle
    // 1 - follow
    // 2 - attack
    private int state = 1;


    private Root m_btRoot = BT.Root();
    private NavMeshAgent agent;

    private PlayerMotionController PMC;

    void Start()
    {
        PMC = GetComponent<PlayerMotionController>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        nearestEnemy = findNearestTag(transform, "Enemy");

        /*
         * process goes as follows:
         * - follow player if no input
         * - if z key is pressed, idle in place
         * - if x key is pressed, continue following player
         * - if c key is pressed, follow then attack nearest enemy
         * 
         * will check every frame for states, toggled by keys.
         * all will see single node, except for c, which will
         * be a selector
         */

        // follow + attack selector
        BTNode attack = BT.RunCoroutine(AttackEnemy);
        BTNode followE = BT.RunCoroutine(FollowEnemy);
        Selector followAttack = BT.Selector(false);
        followAttack.OpenBranch(attack);
        followAttack.OpenBranch(followE);

        // main selector (follow player + followAttack)
        // (idle simply skips tree)
        BTNode followP = BT.RunCoroutine(FollowPlayer);
        Selector selector = BT.Selector(false);
        selector.OpenBranch(followP);
        selector.OpenBranch(followAttack);

        m_btRoot.OpenBranch(selector);
    }

    void Update()
    {
        // collect input
        determineState();

        // updating for serialized fields
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        if (agent.velocity.magnitude > walkAnimThreshold)
            PMC.walk();
        else
            PMC.idle();

        if (state != 0)
            m_btRoot.Tick();
    }

    private void determineState()
    {
        if (Input.GetKey(KeyCode.Z))
            state = 0;
        else if (Input.GetKey(KeyCode.X))
            state = 1;
        else if (Input.GetKey(KeyCode.C))
        {
            // search for nearest enemy, then sic on them!
            nearestEnemy = findNearestTag(transform, "Enemy");
            state = 2;
        }
    }

    public static Transform findNearestTag(Transform entity, string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

        // will return self if none
        Transform closest = entity;
        float closestdist = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(entity.position, enemy.transform.position);
            if (dist < closestdist)
            {
                Debug.Log(enemy);
                closest = enemy.transform;
                closestdist = dist;
            }
        }

        return closest.transform;
    }

    IEnumerator<BTState> FollowPlayer()
    {
        if (state == 1)
        {
            if (Vector3.Distance(transform.position, player.position) > followDistance)
            {
                // follow player with 3.0f padding
                NavMeshHit hit;
                NavMesh.SamplePosition(
                    player.position,
                    out hit, followDistance,
                    NavMesh.AllAreas);
                agent.SetDestination(hit.position);
            }
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    // followAttack selector

    IEnumerator<BTState> AttackEnemy()
    {
        if (Vector3.Distance(transform.position, nearestEnemy.position) < attackRange)
        {
            // play attack anim
            PMC.triggerAttack();
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    IEnumerator<BTState> FollowEnemy()
    {
        // player is not home, so we can follow them
        NavMeshHit hit;
        NavMesh.SamplePosition(
            nearestEnemy.position,
            out hit, 3.0f,
            NavMesh.AllAreas);
        agent.SetDestination(hit.position);

        yield return BTState.Success;
    }
}
