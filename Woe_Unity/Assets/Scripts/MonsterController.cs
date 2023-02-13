using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum MonsternState { Idle, Investigate, Attack, Patrol, Retreat };
    public MonsternState currentState;

    public NavMeshAgent agent;
    float navAgentSpeed_ORG;
    public GameObject player1;

    public LayerMask playerLayerMask;
    public float forwardRayDistance = 10.0f;
    public float forwardRayHeightUp = 0.05f;
    public float forwardRayHeightDown = 0.1f;
    public float backRayDistance = 2.5f;
    public float sideDistance = 5.0f;

    RaycastHit hitForwardData;
    RaycastHit hitBackData;
    RaycastHit hitRightData;
    RaycastHit hitLeftData;

    Vector3 forwardM1;
    Vector3 forwardR1;
    Vector3 forwardL1;
    Vector3 forwardM2;
    Vector3 forwardR2;
    Vector3 forwardL2;

    Vector3 backM;
    Vector3 backR;
    Vector3 backL;
    Vector3 sideR;
    Vector3 sideL;

    Quaternion forwardMAngle1;
    Quaternion forwardRAngle1;
    Quaternion forwardLAngle1;
    Quaternion forwardMAngle2;
    Quaternion forwardRAngle2;
    Quaternion forwardLAngle2;

    Quaternion backMAngle;
    Quaternion backRAngle;
    Quaternion backLAngle;

    Ray rayForwardM1;
    Ray rayForwardR1;
    Ray rayForwardL1;
    Ray rayForwardM2;
    Ray rayForwardR2;
    Ray rayForwardL2;
    
    Ray rayBackM;
    Ray rayBackR;
    Ray rayBackL;
    Ray raySideR;
    Ray raySideL;

    public GameObject[] waypoints;
    public int waypointIndex = 0;
    public int patience = 0;

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        navAgentSpeed_ORG = agent.speed;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        currentState = MonsternState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        SetupRays();
        UpdateState();

        if (Input.GetKeyDown("[0]") && debug)
        {
            ChangeState(MonsternState.Retreat);
        }
        if (Input.GetKeyDown("[3]") && debug)
        {
            ChangeState(MonsternState.Attack);
        }
        if (Input.GetKeyDown("[2]") && debug)
        {
            ChangeState(MonsternState.Investigate);
        }
        if (Input.GetKeyDown("[1]") && debug)
        {
            ChangeState(MonsternState.Idle);
        }
    }

    void ChangeState(MonsternState state)
    {
        currentState = state;
    }

    void SetupRays()
    {
        //Forward ray setup
        forwardMAngle1 = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM1 = forwardMAngle1 * (transform.forward + new Vector3(0, forwardRayHeightUp, 0));
        rayForwardM1 = new Ray(transform.position, forwardM1);

        forwardRAngle1 = Quaternion.AngleAxis(22.5f, new Vector3(0, -1, 0));
        forwardR1 = forwardRAngle1 * (transform.forward + new Vector3(0, forwardRayHeightUp, 0));
        rayForwardR1 = new Ray(transform.position, forwardR1);

        forwardLAngle1 = Quaternion.AngleAxis(-22.5f, new Vector3(0, -1, 0));
        forwardL1 = forwardLAngle1 * (transform.forward + new Vector3(0, forwardRayHeightUp, 0));
        rayForwardL1 = new Ray(transform.position, forwardL1);

        forwardMAngle2 = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM2 = forwardMAngle2 * (transform.forward + new Vector3(0, -forwardRayHeightDown, 0));
        rayForwardM2 = new Ray(transform.position, forwardM2);

        forwardRAngle2 = Quaternion.AngleAxis(22.5f, new Vector3(0, -1, 0));
        forwardR2 = forwardRAngle2 * (transform.forward + new Vector3(0, -forwardRayHeightDown, 0));
        rayForwardR2 = new Ray(transform.position, forwardR2);

        forwardLAngle2 = Quaternion.AngleAxis(-22.5f, new Vector3(0, -1, 0));
        forwardL2 = forwardLAngle2 * (transform.forward + new Vector3(0, -forwardRayHeightDown, 0));
        rayForwardL2 = new Ray(transform.position, forwardL2);

        //Back ray setup
        backMAngle = Quaternion.AngleAxis(0.0f, new Vector3(0, 1, 0));
        backM = backMAngle * -transform.forward;
        rayBackM = new Ray(transform.position, backM);

        backRAngle = Quaternion.AngleAxis(22.5f, new Vector3(0, 1, 0));
        backR = backRAngle * -transform.forward;
        rayBackR = new Ray(transform.position, backR);

        backLAngle = Quaternion.AngleAxis(-22.5f, new Vector3(0, 1, 0));
        backL = backLAngle * -transform.forward;
        rayBackL = new Ray(transform.position, backL);

        //Sides ray setup
        sideR = transform.right;
        raySideR = new Ray(transform.position, backR);

        sideL = -transform.right;
        raySideL = new Ray(transform.position, backL);

        Debug.DrawRay(transform.position, forwardM1 * forwardRayDistance, Color.blue);
        Debug.DrawRay(transform.position, forwardR1 * forwardRayDistance, Color.blue);
        Debug.DrawRay(transform.position, forwardL1 * forwardRayDistance, Color.blue);
        Debug.DrawRay(transform.position, forwardM2 * forwardRayDistance, Color.blue);
        Debug.DrawRay(transform.position, forwardR2 * forwardRayDistance, Color.blue);
        Debug.DrawRay(transform.position, forwardL2 * forwardRayDistance, Color.blue);

        Debug.DrawRay(transform.position, backM * backRayDistance, Color.blue);
        Debug.DrawRay(transform.position, backR * backRayDistance, Color.blue);
        Debug.DrawRay(transform.position, backL * backRayDistance, Color.blue);

        Debug.DrawRay(transform.position, sideR * sideDistance, Color.blue);
        Debug.DrawRay(transform.position, sideL * sideDistance, Color.blue);

        if (((Physics.Raycast(rayForwardM1, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardR1, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardL1, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardM2, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardR2, out hitForwardData, forwardRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayForwardL2, out hitForwardData, forwardRayDistance, playerLayerMask)) ||

            (Physics.Raycast(rayBackM, out hitBackData, backRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayBackR, out hitBackData, backRayDistance, playerLayerMask)) ||
            (Physics.Raycast(rayBackL, out hitBackData, backRayDistance, playerLayerMask)) ||

            (Physics.Raycast(raySideR, out hitRightData, sideDistance, playerLayerMask)) ||
            (Physics.Raycast(raySideL, out hitLeftData, sideDistance, playerLayerMask))) && currentState != MonsternState.Retreat)
        {
            print("Hit");
            ChangeState(MonsternState.Attack);
        }

        Debug.DrawRay(transform.position, forwardM1 * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardR1 * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardL1 * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardM2 * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardR2 * hitForwardData.distance, Color.yellow);
        Debug.DrawRay(transform.position, forwardL2 * hitForwardData.distance, Color.yellow);

        Debug.DrawRay(transform.position, backM * hitBackData.distance, Color.yellow);
        Debug.DrawRay(transform.position, backR * hitBackData.distance, Color.yellow);
        Debug.DrawRay(transform.position, backL * hitBackData.distance, Color.yellow);

        Debug.DrawRay(transform.position, sideR * hitRightData.distance, Color.yellow);
        Debug.DrawRay(transform.position, sideL * hitLeftData.distance, Color.yellow);
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case MonsternState.Idle:
                if (debug)
                {
                    print("In MonsternState.Idle");
                }
                Idle();
                break;

            case MonsternState.Investigate:
                if (debug)
                {
                    print("In MonsternState.Investigate");
                }
                Investigate();
                break;

            case MonsternState.Attack:
                if (debug)
                {
                    print("In MonsternState.Attack");
                }
                Attack();
                break;

            case MonsternState.Patrol:
                if (debug)
                {
                    print("In MonsternState.Patrol");
                }
                Patrol();
                break;

            case MonsternState.Retreat:
                if (debug)
                {
                    print("In MonsternState.Retreat");
                }
                Retreat();
                break;

        }
    }

    void Idle()
    {
        agent.autoBraking = true;
        agent.speed = navAgentSpeed_ORG;
        ChangeState(MonsternState.Patrol);
    }

    void Investigate()
    {
        GameObject closestWaypoint = null;
        closestWaypoint = ClosestWaypoint(player1.transform.position);
        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.speed = navAgentSpeed_ORG;
            ChangeState(MonsternState.Patrol);
        }
    }

    void Attack()
    {
        agent.autoBraking = false;
        agent.speed = navAgentSpeed_ORG;
        agent.SetDestination(player1.transform.position);
    }

    void Patrol()
    {
        if (patience >= 5)
        {
            ChangeState(MonsternState.Investigate);
            patience = 0;
        }
        else
        {
            agent.SetDestination(waypoints[waypointIndex].transform.position);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waypointIndex = Random.Range(0, waypoints.Length);
                patience = patience + 1;
                agent.speed = agent.speed + 5;
            }
        }
        
    }

    void Retreat()
    {
        GameObject closestWaypoint = null;
        closestWaypoint = ClosestWaypoint(transform.position);

        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(MonsternState.Investigate);
        }
    }

    GameObject ClosestWaypoint(Vector3 position)
    {
        GameObject closestWaypoint = null;
        float distance = Mathf.Infinity;
        foreach (GameObject waypoint in waypoints)
        {
            Vector3 diff = waypoint.transform.position - position;
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
