using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
public class MonsterController : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI targetText;

    public enum MonsterState { Idle, Investigate, InvestigateRoom, Attack, Ambush, Patrol, Vent, Retreat };
    public MonsterState currentState;

    public NavMeshAgent agent;
    float navAgentSpeed_ORG;

    public GameObject[] players;
    public int playerTarget = 0;
    public GameObject playerTargeting = null;
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

    public GameObject objectForward;
    public GameObject objectBack;
    public GameObject objectRight;
    public GameObject objectLeft;

    public GameObject[] waypoints;
    public GameObject[] waypointsRoom;
    public int waypointIndex = 0;
    public int waypointRoomIndex = 0;
    public int patience = 0;
    public int patienceMax = 0;
    public GameObject[] ambushSpots;
    public float chanceToAmbush = 0.25f;
    public float ambushWaitTime = 60.0f;
    float ambushWaitTime_ORG;
    public GameObject[] vents;
    public int ventIndex = 0;
    public float chanceToVent = 0.25f;

    //Make Monster go to the start when it idles
    public bool idleToStart = false;
    public bool pausePatrol = false;
    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        navAgentSpeed_ORG = agent.speed;
        ambushWaitTime_ORG = ambushWaitTime;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        players = GameObject.FindGameObjectsWithTag("Player");
        ambushSpots = GameObject.FindGameObjectsWithTag("Ambush Spot");
        vents = GameObject.FindGameObjectsWithTag("Vent");

        currentState = MonsterState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            stateText.text = "Monster's State: " + currentState.ToString();
            targetText.text = "Monster's Target: " + playerTarget.ToString();
        }
        else
        {
            stateText.text = "";
            targetText.text = "";
        }

        SetupRays();
        UpdateState();

        if (Input.GetKeyDown("[0]") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
            ChangeState(MonsterState.Retreat);
        }
        if (Input.GetKeyDown("[1]") && debug)
        {
            ChangeState(MonsterState.Idle);
        }
        if (Input.GetKeyDown("[2]") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
            ChangeState(MonsterState.Investigate);
        }
        if (Input.GetKeyDown("[3]") && debug)
        {
            ChangeState(MonsterState.Attack);
        }
        if (Input.GetKeyDown("[4]") && debug)
        {
            ChangeState(MonsterState.Vent);
        }
        if (Input.GetKeyDown("[5]") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
        }
        if (Input.GetKeyDown("[6]") && debug)
        {
            ventIndex = Random.Range(0, vents.Length);
        }
    }

    public void ChangeState(MonsterState state)
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
            (Physics.Raycast(rayForwardL2, out hitForwardData, forwardRayDistance, playerLayerMask))) && currentState != MonsterState.Retreat)
        {
            if (debug)
            {
                print("Hit");
            }

            objectForward = hitForwardData.collider.gameObject;

            playerTargeting = objectForward;

            ChangeState(MonsterState.Attack);
        }

        if (((Physics.Raycast(rayBackM, out hitBackData, backRayDistance, playerLayerMask)) ||
        (Physics.Raycast(rayBackR, out hitBackData, backRayDistance, playerLayerMask)) ||
        (Physics.Raycast(rayBackL, out hitBackData, backRayDistance, playerLayerMask))) && currentState != MonsterState.Retreat)
        {
            if (debug)
            {
                print("Hit");
            }

            objectBack = hitBackData.collider.gameObject;

            playerTargeting = objectBack;

            ChangeState(MonsterState.Attack);
        }

        if (((Physics.Raycast(raySideR, out hitRightData, sideDistance, playerLayerMask))) && currentState != MonsterState.Retreat)
        {
            if (debug)
            {
                print("Hit");
            }

            objectRight = hitRightData.collider.gameObject;

            playerTargeting = objectRight;

            ChangeState(MonsterState.Attack);
        }

        if (((Physics.Raycast(raySideL, out hitLeftData, sideDistance, playerLayerMask))) && currentState != MonsterState.Retreat)
        {
            if (debug)
            {
                print("Hit");
            }

            objectLeft = hitLeftData.collider.gameObject;

            playerTargeting = objectLeft;

            ChangeState(MonsterState.Attack);
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
            case MonsterState.Idle:
                if (debug)
                {
                    print("In MonsterState.Idle");
                }
                Idle();
                break;

            case MonsterState.Investigate:
                if (debug)
                {
                    print("In MonsterState.Investigate");
                }
                Investigate();
                break;

            case MonsterState.InvestigateRoom:
                if (debug)
                {
                    print("In MonsterState.InvestigateRoom");
                }
                InvestigateRoom();
                break;

            case MonsterState.Attack:
                if (debug)
                {
                    print("In MonsterState.Attack");
                }
                Attack();
                break;

            case MonsterState.Ambush:
                if (debug)
                {
                    print("In MonsterState.Ambush");
                }
                Ambush();
                break;

            case MonsterState.Patrol:
                if (debug)
                {
                    print("In MonsterState.Patrol");
                }
                Patrol();
                break;

            case MonsterState.Vent:
                if (debug)
                {
                    print("In MonsterState.Vent");
                }
                Vent();
                break;

            case MonsterState.Retreat:
                if (debug)
                {
                    print("In MonsterState.Retreat");
                }
                Retreat();
                break;

        }
    }

    void Idle()
    {
        patienceMax = Random.Range(1, 5);
        playerTarget = 0;
        playerTargeting = null;
        agent.autoBraking = true;
        agent.speed = navAgentSpeed_ORG;
        if(idleToStart)
        {
            waypointIndex = 0;
        }
        waypointRoomIndex = 0;
        waypointsRoom = new GameObject[0];

        if (pausePatrol)
        {
            ChangeState(MonsterState.Idle);
        }
        else
        {

            ChangeState(MonsterState.Patrol);
        }
    }

    void Investigate()
    {
        GameObject closestWaypoint = null;

        closestWaypoint = ClosestWaypoint(playerTargeting.transform.position, waypoints);
        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.autoBraking = false;
            if (closestWaypoint.gameObject.layer == LayerMask.NameToLayer("Room"))
            {
                int index = 0;
                waypointsRoom = new GameObject[closestWaypoint.transform.childCount];
                foreach (Transform child in closestWaypoint.transform)
                {
                    if (child.tag == "Waypoint Room")
                    {
                        waypointsRoom[index] = child.gameObject;
                        index += 1;
                    }
                }

                ChangeState(MonsterState.InvestigateRoom);
            }
            else
            {
                ChangeState(MonsterState.Idle);
            }
        }
    }

    void InvestigateRoom()
    {
        if (waypointRoomIndex >= waypointsRoom.Length)
        {
            if (Random.value < chanceToVent)
            {
                ventIndex = Random.Range(0, vents.Length);
                ChangeState(MonsterState.Vent);
            }
            else
            {
                ChangeState(MonsterState.Idle);
            }
        }
        else
        {
            agent.SetDestination(waypointsRoom[waypointRoomIndex].transform.position);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waypointRoomIndex += 1;
            }
        }


    }

    void Attack()
    {
        agent.autoBraking = false;
        agent.speed = navAgentSpeed_ORG;
        agent.SetDestination(playerTargeting.transform.position);
    }

    void Ambush()
    {
        GameObject closestWaypoint = null;

        closestWaypoint = ClosestWaypoint(playerTargeting.transform.position, ambushSpots);
        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestWaypoint.transform.GetChild(0).localPosition), Time.deltaTime * 2.5f);

            ambushWaitTime -= Time.deltaTime;
            if (ambushWaitTime < 0)
            {
                ambushWaitTime = ambushWaitTime_ORG;
                ChangeState(MonsterState.Idle);
            }
        }
    }

    void Patrol()
    {
        if (pausePatrol)
        {
            ChangeState(MonsterState.Idle);
        }

        if (patience >= patienceMax)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
            if (Random.value < chanceToAmbush)
            {
                ChangeState(MonsterState.Ambush);
            }
            else if(Random.value < chanceToVent)
            {
                ventIndex = Random.Range(0, vents.Length);
                ChangeState(MonsterState.Vent);
            }
            else
            {
                ChangeState(MonsterState.Investigate);
            }
            patience = 0;
        }
        else
        {
            agent.SetDestination(waypoints[waypointIndex].transform.position);
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waypointIndex = Random.Range(0, waypoints.Length);
                patience = patience + 1;
                agent.speed = agent.speed + 1.0f;
            }
        }
        
    }

    void Vent()
    {
        agent.SetDestination(vents[ventIndex].transform.position);
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    void Retreat()
    {
        GameObject closestWaypoint = null;
        closestWaypoint = ClosestWaypoint(transform.position, waypoints);

        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(MonsterState.Investigate);
        }
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

    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.tag == "Player")
    //    {
    //        hit.gameObject.GetComponent<PlayerController>().TakeDamage(50);
    //    }
    //}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(50);
        }

        if ((collision.gameObject.tag == "Enemy") && currentState == MonsterState.Attack)
        {
            Destroy(collision.gameObject);
        }
    }
}
