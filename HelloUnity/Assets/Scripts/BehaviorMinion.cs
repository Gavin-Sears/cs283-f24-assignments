using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using BTAI;

public class BehaviorMinon : MonoBehaviour
{
    [SerializeField]
    private Transform wanderRange;  // Set to a sphere
    [SerializeField]
    private Transform homeRange; // Set to safe area for player
    [SerializeField]
    private Transform player; // What minion will attack/follow
    [SerializeField]
    private float attackRange = 1.0f; // Range within which minion will attack

    private Root m_btRoot = BT.Root();
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        /*
         * process goes as follows:
         * - attack if we are close
         * - if we can't attack, follow the player if they are not home
         * - if we can't follow, flee to wander location, unless we are already in range
         * - if we don't need to flee, wander in range
         * wander will always be successful
         */
        BTNode attack = BT.RunCoroutine(AttackPlayer);
        BTNode follow = BT.RunCoroutine(FollowPlayer);
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
        m_btRoot.Tick();
    }

    IEnumerator<BTState> AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            // minion is inside attack range
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    IEnumerator<BTState> FollowPlayer()
    {
        if (Vector3.Distance(player.position, homeRange.position) >= homeRange.localScale.x)
        {
            // player is not home, so we can follow them
            yield return BTState.Success;
        }

        yield return BTState.Failure;
    }

    // assume splayer is not home
    IEnumerator<BTState> Flee()
    {
        if (Vector3.Distance(transform.position, wanderRange.position) >= wanderRange.localScale.x)
        {
            // we are outside of range of wander sphere
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

        yield return BTState.Success;
    }
}