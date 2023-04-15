using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.Netcode;

public class MonsterController : NetworkBehaviour
{
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI targetText;

    public enum MonsterState { Idle, Investigate, InvestigateRoom, Attack, Ambush, Patrol, Vent, Retreat, Follow, Caution };
    public MonsterState currentState;

    public enum MonsterIntelligence { Dumb, Incompetent, Competent, Smart, ApexPredator};
    public MonsterIntelligence intelligence;

    public NavMeshAgent agent;
    float navAgentSpeed_ORG;
    float additiveSpeed;
    float speedModifer;

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

    GameObject objectForward;
    GameObject objectBack;
    GameObject objectRight;
    GameObject objectLeft;

    public GameObject[] waypoints;
    public GameObject[] waypointsRoom;
    public int waypointIndex = 0;
    public int waypointRoomIndex = 0;
    public int patience = 0;
    public int patienceMax = 0;
    int patienceMaxRangeMax = 0;
    int patienceMaxRangeMin = 0;
    public GameObject[] ambushSpots;
    public float chanceToAmbush = 0.25f;
    public float ambushWaitTime = 60.0f;
    float ambushWaitTime_ORG;
    public GameObject[] vents;
    public int ventIndex = 0;
    public float chanceToVent = 0.25f;

    bool lingerInRoom;
    float lingerWaitTime = 5.0f;
    float lingerWaitTime_ORG;

    float restVentTime = 5.0f;
    float restVentTime_ORG;

    float cautionWaitTime = 3.0f;
    float cautionWaitTime_ORG;

    float colideWaitTime = 2.0f;
    float colideWaitTime_ORG;

    public float remainingWaypointDistance = 1.5f; //0.5f;

    //Make Monster go to the start when it idles
    public bool idleToStart = false;
    public bool pausePatrol = false;
    public bool debug = false;

    public bool decon = false;
    [SerializeField]
    private float menace = 60.0f;
    public bool menaceSystemActive = true;

    // Start is called before the first frame update
    //public override void OnNetworkSpawn()
    void Start()
    {
        navAgentSpeed_ORG = agent.speed;
        ambushWaitTime_ORG = ambushWaitTime;
        lingerWaitTime_ORG = lingerWaitTime;
        restVentTime_ORG = restVentTime;
        cautionWaitTime_ORG = cautionWaitTime;
        colideWaitTime_ORG = colideWaitTime;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        players = GameObject.FindGameObjectsWithTag("Player");
        ambushSpots = GameObject.FindGameObjectsWithTag("Ambush Spot");
        vents = GameObject.FindGameObjectsWithTag("Vent");

        currentState = MonsterState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

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
        UpdateStates();

        if (Input.GetKeyDown("r") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
            ChangeState(MonsterState.Retreat);
        }
        if (Input.GetKeyDown("i") && debug)
        {
            ChangeState(MonsterState.Idle);
        }
        if (Input.GetKeyDown("t") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
            ChangeState(MonsterState.Investigate);
        }
        if (Input.GetKeyDown("u") && debug)
        {
            ChangeState(MonsterState.Attack);
        }
        if (Input.GetKeyDown("y") && debug)
        {
            ChangeState(MonsterState.Vent);
        }
        if (Input.GetKeyDown("p") && debug)
        {
            playerTarget = Random.Range(0, players.Length);
            playerTargeting = players[playerTarget];
        }
        if (Input.GetKeyDown("h") && debug)
        {
            ventIndex = Random.Range(0, vents.Length);
        }

        if (decon)
        {
            foreach (GameObject vent in vents)
            {
                vent.SetActive(false);
            }
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

            objectForward = hitForwardData.collider.gameObject;

            if (objectForward.GetComponent<PlayerController>().playerState == PlayerController.PlayerState.Alive)
            {
                playerTargeting = objectForward;
                ChangeState(MonsterState.Attack);
            }

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

    void UpdateStates()
    {
        switch (intelligence)
        {
            case MonsterIntelligence.Dumb:
                //Add modifiers to monster's ray detection, speed and persistence
                patienceMaxRangeMax = 10;
                patienceMaxRangeMin = 8;
                chanceToVent = 0.5f;
                speedModifer = -0.05f;
                additiveSpeed = 0.25f;
                lingerInRoom = false;
                break;

            case MonsterIntelligence.Incompetent:
                //Add modifiers to monster's ray detection, speed and persistence
                patienceMaxRangeMax = 8;
                patienceMaxRangeMin = 5;
                chanceToVent = 0.45f;
                speedModifer = -0.025f;
                additiveSpeed = 0.5f;
                lingerInRoom = false;
                break;

            case MonsterIntelligence.Competent:
                //Add modifiers to monster's ray detection, speed and persistence
                patienceMaxRangeMax = 5;
                patienceMaxRangeMin = 1;
                chanceToVent = 0.35f;
                speedModifer = 0.0f;
                additiveSpeed = 1.0f;
                lingerInRoom = true;
                break;

            case MonsterIntelligence.Smart:
                //Add modifiers to monster's ray detection, speed and persistence
                patienceMaxRangeMax = 2;
                patienceMaxRangeMin = 1;
                chanceToVent = 0.25f;
                speedModifer = 0.05f;
                additiveSpeed = 2.0f;
                lingerInRoom = true;
                break;

            case MonsterIntelligence.ApexPredator:
                //Add modifiers to monster's ray detection, speed and persistence
                patienceMaxRangeMax = 1;
                patienceMaxRangeMin = 1;
                chanceToVent = 0.0f;
                chanceToAmbush = 0.0f;
                speedModifer = 0.1f;
                additiveSpeed = 2.5f;
                lingerInRoom = true;
                break;

        }

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

            case MonsterState.Follow:
                if (debug)
                {
                    print("In MonsterState.Follow");
                }
                Follow();
                break;

            case MonsterState.Caution:
                if (debug)
                {
                    print("In MonsterState.Caution");
                }
                Caution();
                break;

        }
    }

    void Idle()
    {
        patienceMax = Random.Range(patienceMaxRangeMin, patienceMaxRangeMax);
        playerTarget = 0;
        playerTargeting = null;
        agent.autoBraking = true;
        agent.speed = navAgentSpeed_ORG + speedModifer;
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

        if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
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
        agent.speed = navAgentSpeed_ORG;

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
            if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
            {
                if (lingerInRoom)
                {
                    lingerWaitTime -= Time.deltaTime;
                    if (lingerWaitTime < 0.0f)
                    {
                        lingerWaitTime = lingerWaitTime_ORG;
                        waypointRoomIndex += 1;
                    }
                }
                else
                {
                    waypointRoomIndex += 1;
                }
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
        agent.speed = navAgentSpeed_ORG;

        GameObject closestWaypoint = null;

        closestWaypoint = ClosestWaypoint(playerTargeting.transform.position, ambushSpots);
        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestWaypoint.transform.GetChild(0).localPosition), Time.deltaTime * 2.5f);

            ambushWaitTime -= Time.deltaTime;
            if (ambushWaitTime < 0.0f)
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
                if (decon)
                {
                    ChangeState(MonsterState.Investigate);
                }
                else
                {
                    ChangeState(MonsterState.Ambush);
                }
                
            }
            else if(Random.value < chanceToVent)
            {
                if (decon)
                {
                    ChangeState(MonsterState.Investigate);
                }
                else
                {
                    ventIndex = Random.Range(0, vents.Length);
                    ChangeState(MonsterState.Vent);
                }
            }
            else
            {
                if(playerTargeting.GetComponent<MovementController>().safe)
                {
                    ChangeState(MonsterState.Follow);
                }
                else
                {
                    if ( (menaceSystemActive) && (playerTargeting.GetComponent<PlayerController>().pressure > menace))
                    {
                        playerTargeting.GetComponent<PlayerController>().pressure = 0;
                        ChangeState(MonsterState.Patrol);
                    }
                    else
                    {
                        ChangeState(MonsterState.Investigate);
                    }
                }
                
            }
            patience = 0;
        }
        else
        {
            agent.SetDestination(waypoints[waypointIndex].transform.position);
            if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
            {
                waypointIndex = Random.Range(0, waypoints.Length);
                patience = patience + 1;
                agent.speed = agent.speed + additiveSpeed;
            }
        }
        
    }

    void Vent()
    {
        restVentTime -= Time.deltaTime;
        if (restVentTime < 0.0f)
        {
            restVentTime = restVentTime_ORG;
            foreach (GameObject vent in vents)
            {
                vent.SetActive(true);
            }
        }

        agent.SetDestination(vents[ventIndex].transform.position);
        if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
        {
            if (restVentTime == restVentTime_ORG)
            {

                ChangeState(MonsterState.Idle);
            }
        }
    }

    void Retreat()
    {
        GameObject closestWaypoint = null;
        closestWaypoint = ClosestWaypoint(transform.position, waypoints);

        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
        {
            ChangeState(MonsterState.Investigate);
        }
    }

    void Follow()
    {
        /*State used for monster to follow around the player but not necessarily try to attack them, 
        intended to be used when player is solving puzzles and the monster is not permitted to attack*/

        GameObject closestWaypoint = null;
        closestWaypoint = ClosestWaypoint(playerTargeting.transform.position, vents);
        agent.SetDestination(closestWaypoint.transform.position);

        if (!agent.pathPending && agent.remainingDistance < remainingWaypointDistance)
        {
            closestWaypoint.SetActive(false);
            ChangeState(MonsterState.Vent);
        }
    }

    void Caution()
    {
        /*State used for monster to slowly approch the player when they are holding certain items in front of them, 
        intended to be used as a way for the player to have a little space when the monster is chasing them*/

        cautionWaitTime -= Time.deltaTime;
        if (cautionWaitTime < 0.0f)
        {
            agent.speed = navAgentSpeed_ORG;
            cautionWaitTime = cautionWaitTime_ORG;
        }
        else
        {
            agent.speed = 1.5f;
        }

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(50);
        }

        if (collision.gameObject.tag == "Bot")
        {
            if (collision.gameObject.GetComponent<BotController>().shutDown)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                colideWaitTime -= Time.deltaTime;
                if (colideWaitTime < 0.0f)
                {
                    Destroy(collision.gameObject);
                    colideWaitTime = colideWaitTime_ORG;
                }
            }
        }
    }
}
