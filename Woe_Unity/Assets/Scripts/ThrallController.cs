using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThrallController : MonoBehaviour
{
    public enum ThrallState { Idle, Wander, Stop, SlowDown, Retreat, Attack };
    public ThrallState currentState;

    public GameObject monster;

    public NavMeshAgent agent;
    float navAgentRadius_ORG;
    float navAgentSpeed_ORG;

    public float followTime = 10.0f;
    float followTime_ORG;

    public GameObject[] waypoints;
    public int waypointIndex = 0;

    public float forwardRayDistance = 10.0f;
    public float forwardRayHeight = 0.00f;
    public float forwardRayWidth = 22.5f;

    RaycastHit hitForwardData;

    Vector3 forwardM;
    Vector3 forwardR;
    Vector3 forwardL;

    Quaternion forwardMAngle;
    Quaternion forwardRAngle;
    Quaternion forwardLAngle;

    Ray rayForwardM;
    Ray rayForwardR;
    Ray rayForwardL;

    public GameObject objectForward;

    public GameObject playerTargeting = null;
    public LayerMask playerLayerMask;
    public LayerMask monsterLayerMask;
    public LayerMask botLayerMask;

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        followTime_ORG = followTime;
        navAgentRadius_ORG = agent.radius;
        navAgentSpeed_ORG = agent.speed;
        currentState = ThrallState.Idle;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        monster = GameObject.FindGameObjectWithTag("Monster");
    }

    // Update is called once per frame
    void Update()
    {
        SetupRays();
        UpdateState();
    }

    void SetupRays()
    {
        //Forward ray setup
        forwardMAngle = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM = forwardMAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardM = new Ray(transform.position, forwardM);

        forwardRAngle = Quaternion.AngleAxis(forwardRayWidth, new Vector3(0, -1, 0));
        forwardR = forwardRAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardR = new Ray(transform.position, forwardR);

        forwardLAngle = Quaternion.AngleAxis(-forwardRayWidth, new Vector3(0, -1, 0));
        forwardL = forwardLAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardL = new Ray(transform.position, forwardL);

        Debug.DrawRay(transform.position, forwardM * forwardRayDistance, Color.cyan);
        Debug.DrawRay(transform.position, forwardR * forwardRayDistance, Color.cyan);
        Debug.DrawRay(transform.position, forwardL * forwardRayDistance, Color.cyan);

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, playerLayerMask))))
        {
            objectForward = hitForwardData.collider.gameObject;

        }

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, monsterLayerMask))))
        {
            ChangeState(ThrallState.SlowDown);
        }

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, botLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, botLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, botLayerMask))))
        {
            ChangeState(ThrallState.SlowDown);
        }

        Debug.DrawRay(transform.position, forwardM * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardR * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardL * hitForwardData.distance, Color.yellow);


    }

    void UpdateState()
    {
        switch (currentState)
        {
            case ThrallState.Idle:
                if (debug)
                {
                    print("In ThrallState.Idle");
                }
                Idle();
                break;

            case ThrallState.Stop:
                if (debug)
                {
                    print("In ThrallState.Stop");
                }
                Stop();
                break;

            case ThrallState.SlowDown:
                if (debug)
                {
                    print("In ThrallState.SlowDown");
                }
                SlowDown();
                break;

            case ThrallState.Wander:
                if (debug)
                {
                    print("In ThrallState.Wander");
                }
                Wander();
                break;
            case ThrallState.Retreat:
                if (debug)
                {
                    print("In ThrallState.Retreat");
                }
                Retreat();
                break;

            case ThrallState.Attack:
                if (debug)
                {
                    print("In ThrallState.Attack");
                }
                Attack();
                break;
        }
    }

    public void ChangeState(ThrallState state)
    {
        currentState = state;
    }

    void Idle()
    {
        agent.speed = navAgentSpeed_ORG;
        agent.radius = navAgentRadius_ORG;
        waypointIndex = 0;
        ChangeState(ThrallState.Wander);
    }

    void Stop()
    {
        agent.speed = 0.0f;
        agent.radius = 0.1f;

        ChangeState(ThrallState.Idle);
    }

    void SlowDown()
    {
        agent.speed = navAgentSpeed_ORG / 2;
        agent.radius = navAgentRadius_ORG / 2;

        ChangeState(ThrallState.Idle);
    }

    void Wander()
    {

    }

    void Retreat()
    {
        //agent.speed = 5.0f;
        //agent.radius = 0.1f;

        //GameObject closestWaypoint = null;
        //closestWaypoint = ClosestWaypoint(monster.transform.position, waypoints);

        //agent.SetDestination(closestWaypoint.transform.position);

        //if (!agent.pathPending && agent.remainingDistance < 0.5)
        //{
        //    ChangeState(ThrallState.Idle);
        //}


        agent.SetDestination(monster.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5)
        {
            ChangeState(ThrallState.Idle);
        }
    }

    void Attack()
    {
        agent.speed = navAgentSpeed_ORG;
        agent.SetDestination(playerTargeting.transform.position);
    }

    GameObject ClosestWaypoint(Vector3 position_, GameObject[] waypoints_)
    {
        GameObject closestWaypoint = null;
        float distance = Mathf.Infinity;
        foreach (GameObject waypoint in waypoints_)
        {
            Vector3 diff = waypoint.transform.position - position_;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestWaypoint = waypoint;
                distance = curDistance;
            }
        }

        return closestWaypoint;
    }
}
