using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    public enum BotState { Idle, Patrol, Attack};
    public BotState currentState;

    public NavMeshAgent agent;

    public GameObject[] waypoints;
    public int waypointIndex = 0;

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        currentState = BotState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        SetupRays();
        UpdateState();
    }

    void SetupRays()
    {

    }

    void UpdateState()
    {
        switch (currentState)
        {
            case BotState.Idle:
                if (debug)
                {
                    print("In BotState.Idle");
                }
                Idle();
                break;

            case BotState.Attack:
                if (debug)
                {
                    print("In BotState.Attack");
                }
                Attack();
                break;

            case BotState.Patrol:
                if (debug)
                {
                    print("In BotState.Patrol");
                }
                Patrol();
                break;

        }
    }

    public void ChangeState(BotState state)
    {
        currentState = state;
    }

    void Idle()
    {
        waypointIndex = 0;
        ChangeState(BotState.Patrol);
    }

    void Attack()
    {

    }

    void Patrol()
    {
        if (waypointIndex >= waypoints.Length)
        {
            ChangeState(BotState.Idle);
        }
        else
        {
            agent.SetDestination(waypoints[waypointIndex].transform.position);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waypointIndex += 1;
            }
        }

    }
}
