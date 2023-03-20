using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BotController : NetworkBehaviour
{
    public enum BotState { Idle, Patrol, Attack, MoveAway};
    public BotState currentState;
    public GameObject monster;

    public NavMeshAgent agent;

    public GameObject[] waypoints;
    public int waypointIndex = 0;

    public float forwardRayDistance = 10.0f;
    public float forwardRayHeight = 0.00f;

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

    public bool debug = false;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        currentState = BotState.Idle;
        monster = GameObject.FindGameObjectWithTag("Monster");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        SetupRays();
        UpdateState();
    }

    void SetupRays()
    {
        //Forward ray setup
        forwardMAngle = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM = forwardMAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardM = new Ray(transform.position, forwardM);

        forwardRAngle = Quaternion.AngleAxis(22.5f, new Vector3(0, -1, 0));
        forwardR = forwardRAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardR = new Ray(transform.position, forwardR);

        forwardLAngle = Quaternion.AngleAxis(-22.5f, new Vector3(0, -1, 0));
        forwardL = forwardLAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardL = new Ray(transform.position, forwardL);

        Debug.DrawRay(transform.position, forwardM * forwardRayDistance, Color.cyan);
        Debug.DrawRay(transform.position, forwardR * forwardRayDistance, Color.cyan);
        Debug.DrawRay(transform.position, forwardL * forwardRayDistance, Color.cyan);

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, playerLayerMask))) )
        {
            if (debug)
            {
                print("Hit");
            }

            objectForward = hitForwardData.collider.gameObject;

            playerTargeting = objectForward;

            ChangeState(BotState.Attack);
        }

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, monsterLayerMask))))
        {
            objectForward = hitForwardData.collider.gameObject;

            ChangeState(BotState.MoveAway);
        }

        Debug.DrawRay(transform.position, forwardM * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardR * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardL * hitForwardData.distance, Color.yellow);
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

            case BotState.MoveAway:
                if (debug)
                {
                    print("In BotState.MoveAway");
                }
                MoveAway();
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

    void MoveAway()
    {
        
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

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{
        //    collision.gameObject.GetComponent<PlayerController>().TakeDamage(25);
        //}

        //if ((collision.gameObject.tag == "Monster"))
        //{
        //    Destroy(this.gameObject);
        //}
    }
}
