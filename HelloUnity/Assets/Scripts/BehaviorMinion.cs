using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI;

public class BehaviorMinon : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField]
    private float agentAcceleration = 3.0f;
    [SerializeField]
    private float agentSpeed = 4.0f;
    [SerializeField]
    private float waitTime = 0.0f; // time stopped while wandering
    private float stoppedTime = 0.0f;
    [SerializeField]
    private float attackRange = 3.0f; // Range within which minion will attack

    // when above this speed, will do walk anim
    [SerializeField]
    private float walkAnimThreshold = 0.2f;

    [Header("environment")]
    [SerializeField]
    private Transform wanderRange;  // Set to a sphere
    [SerializeField]
    private Transform homeRange; // Set to safe area for player
    [SerializeField]
    private Transform player; // What minion will attack/follow

    private Transform nearestAlly;


    private Root m_btRoot = BT.Root();
    private NavMeshAgent agent;

    private PlayerMotionController PMC;

    void Start()
    {
        PMC = GetComponent<PlayerMotionController>();
        agent = GetComponent<NavMeshAgent>();

        nearestAlly = BehaviorUnique.findNearestTag(transform, "Ally");

        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        /*
         * process goes as follows:
         * - attack if we are close
         * - if we can't attack, follow the player if they are not home
         * - if we can't follow, flee to wander location, unless we are already in range
         * - if we don't need to flee, wander in range
         * wander will always be successful
         */
        BTNode attack = BT.RunCoroutine(AttackAlly);
        BTNode follow = BT.RunCoroutine(FollowAlly);
        BTNode flee = BT.RunCoroutine(Flee);
        BTNode moveTo = BT.RunCoroutine(MoveToRandom);

        Selector selector = BT.Selector(false);
        selector.OpenBranch(attack);
        selector.OpenBranch(follow);
        selector.OpenBranch(flee);
        selector.OpenBranch(moveTo);

        m_btRoot.OpenBranch(selector);
    }

    void Update()
    {
        // updating for serialized fields
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;

        if (agent.velocity.magnitude > walkAnimThreshold)
            PMC.walk();
        else
            PMC.idle();

        m_btRoot.Tick();
    }

    IEnumerator<BTState> AttackAlly()
    {
        if (Vector3.Distance(transform.position, nearestAlly.position) < attackRange)
        {
            // play attack anim
            PMC.triggerAttack();
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    IEnumerator<BTState> FollowAlly()
    {
        if (Time.frameCount % 5 == 0)
        {
            nearestAlly = BehaviorUnique.findNearestTag(transform, "Ally");
        }

        if (Vector3.Distance(nearestAlly.position, homeRange.position) >= homeRange.localScale.x)
        {
            // nearest ally character is not home, so we can follow them
            NavMeshHit hit;
            NavMesh.SamplePosition(
                nearestAlly.position,
                out hit, 1.0f,
                NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    // assumes player is not home
    IEnumerator<BTState> Flee()
    {
        if (Vector3.Distance(transform.position, wanderRange.position) >= wanderRange.localScale.x)
        {
            if (agent.velocity.magnitude > walkAnimThreshold)
            {
                // we are outside of range of wander sphere
                NavMeshHit hit;
                NavMesh.SamplePosition(
                    wanderRange.position,
                    out hit, 1.0f,
                    NavMesh.AllAreas);
                agent.SetDestination(hit.position);
            }
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    IEnumerator<BTState> MoveToRandom()
    {
        Vector3 target;
        Wander.RandomPointOnTerrain(
           wanderRange.position, wanderRange.localScale.x, out target);
        agent.SetDestination(target);

        // wait for agent to reach destination
        while (agent.remainingDistance > 0.1f)
        {
            yield return BTState.Continue;
        }

        // agent waits at destination
        stoppedTime = 0.0f;
        while (stoppedTime < waitTime)
        {
            stoppedTime += Time.deltaTime;
            yield return BTState.Continue;
        }

        yield return BTState.Success;
    }
}