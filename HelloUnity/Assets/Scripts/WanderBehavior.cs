using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI;

public class WanderBehavior : MonoBehaviour
{
    public Transform wanderRange;  // Set to a sphere
    private Root m_btRoot = BT.Root();
    private NavMeshAgent agent;
    private PlayerMotionController PMC;

    [SerializeField]
    private float waitTime = 0.0f;
    private float stoppedTime = 0.0f;

    // when above this speed, will do walk anim
    [SerializeField]
    private float walkAnimThreshold = 0.2f;

    void Start()
    {
        PMC = GetComponent<PlayerMotionController>();
        agent = GetComponent<NavMeshAgent>();
        BTNode moveTo = BT.RunCoroutine(MoveToRandom);

        Sequence sequence = BT.Sequence();
        sequence.OpenBranch(moveTo);

        m_btRoot.OpenBranch(sequence);
    }

    void Update()
    {
        m_btRoot.Tick();

        if (agent.velocity.magnitude > walkAnimThreshold)
            PMC.walk();
        else
            PMC.idle();
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