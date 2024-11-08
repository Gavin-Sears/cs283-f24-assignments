using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
    [Header("NPC range")]
    [SerializeField]
    private Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField]
    private float range = 10.0f;
    // how close agent gets to destination
    [SerializeField]
    private float maxCloseness = 2.0f;

    [Header("NPC parameters")]
    // amount of time agent pauses before moving again
    [SerializeField]
    private float pauseTime = 2.0f;
    [SerializeField]
    private float agentAcceleration = 3.0f;
    [SerializeField]
    private float agentSpeed = 4.0f;

    private NavMeshAgent agent;
    private Vector3 destination;
    private float timeStopped = 0.0f;
    private PlayerMotionController PMC;

    private Vector3 defaultPos;

    // Start is called before the first frame update
    void Start()
    {
        PMC = GetComponent<PlayerMotionController>();
        agent = GetComponent<NavMeshAgent>();

        destination = transform.position;
        agent.SetDestination(destination);

        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;
        agent.stoppingDistance = maxCloseness;

        defaultPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        PMC.walk();
        // updating for serialized fields
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;
        agent.stoppingDistance = maxCloseness;

        if (agent.velocity.magnitude < 0.0001f &&
            Vector3.Distance(transform.position, destination) < maxCloseness)
        {
            PMC.idle();
            timeStopped += Time.deltaTime;
            if (timeStopped > pauseTime)
            {
                changeDestination();
                timeStopped = 0.0f;
                PMC.walk();
            }
        }
    }

    private void changeDestination()
    {
        // Unity docs example for sampling random point
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                agent.SetDestination(destination);
                return;
            }
        }
        destination = defaultPos;
        agent.SetDestination(destination);
        return;
    }
}
