using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class BotController : NetworkBehaviour
{
    public enum BotType { DoctorBot, NurseBot, SecurityBot};
    public BotType botType;

    public enum BotState { Idle, Patrol, Stop, SlowDown, Task, ShutDown};
    public BotState currentState;

    public GameObject monster;

    public NavMeshAgent agent;
    float navAgentRadius_ORG;
    float navAgentSpeed_ORG;

    [SerializeField]
    private float followWaitTime = 15.0f;
    float followWaitTime_ORG;

    public GameObject[] waypoints;
    public int waypointIndex = 0;
    public Vector3 startingPostion;

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
    public LayerMask enemyLayerMask;

    [SerializeField]
    private GameObject itemToSpawn;
    public GameObject objectSpawner;
    public GameObject[] itemTable;
    [SerializeField]
    private Vector3 itemScale = new Vector3(0.25f, 0.25f, 0.25f);

    [SerializeField]
    private float itemSpawnTime = 180.0f; // 3 Minutes
    float itemSpawnTime_ORG;

    [SerializeField]
    private float chanceToSpawn = 0.5f; // 50%

    public bool debug = false;

    public bool shutDown = false;

    public GameObject lamp;
    public Material activeMaterial;
    public Material deactiveMaterial;
    Material defaultMaterial;


    // Start is called before the first frame update
    void Start()
    {
        itemSpawnTime_ORG = itemSpawnTime;
        followWaitTime_ORG = followWaitTime;
        navAgentRadius_ORG = agent.radius;
        navAgentSpeed_ORG = agent.speed;
        currentState = BotState.Idle;
        monster = GameObject.FindGameObjectWithTag("Monster");
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
        startingPostion = this.transform.position;
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
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, playerLayerMask))) )
        {
            objectForward = hitForwardData.collider.gameObject;

            if (botType == BotType.DoctorBot)
            {
                ChangeState(BotState.Stop);

                if (agent.velocity == Vector3.zero)
                {

                    this.GetComponent<MeshRenderer>().material = objectForward.GetComponent<PlayerController>().selectMaterial;

                    if (shutDown)
                    {
                        objectForward.GetComponent<PlayerController>().infoText.text = "Doctor Bot has been shutdown";
                    }
                    else
                    {
                        objectForward.GetComponent<PlayerController>().requestHealth = true;
                        objectForward.GetComponent<PlayerController>().infoText.text = "Use E: Heal";
                    }
                }
                
            }

            if (botType == BotType.NurseBot)
            {
                ChangeState(BotState.Stop);
            }

            if ((botType == BotType.SecurityBot) && (!shutDown))
            {
                if ((monster.GetComponent<MonsterController>().currentState != MonsterController.MonsterState.Attack) && (Vector3.Distance(monster.transform.position, this.transform.position) > 25.0f))
                {
                    monster.GetComponent<MonsterController>().playerTargeting = this.gameObject;
                    monster.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Investigate;
                }
                else
                {
                    monster.GetComponent<MonsterController>().playerTargeting = objectForward;
                    monster.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Attack;
                }

                ChangeState(BotState.Task);
            }

        }
        else
        {
            if (botType == BotType.SecurityBot)
            {
                lamp.GetComponent<MeshRenderer>().material = deactiveMaterial;
            }
            this.GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, monsterLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, monsterLayerMask))) )
        {
            objectForward = hitForwardData.collider.gameObject;

            if ((botType == BotType.NurseBot) && (botType == BotType.DoctorBot))
            {
                ChangeState(BotState.Stop);
            }

            if ((botType == BotType.SecurityBot) && (!shutDown))
            {
                ChangeState(BotState.Task);
            }

            if (shutDown)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (botType == BotType.SecurityBot)
            {
                lamp.GetComponent<MeshRenderer>().material = deactiveMaterial;
            }
        }

        if (((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, botLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, botLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, botLayerMask))) ||

            ((Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, enemyLayerMask)) ||
            (Physics.Raycast(rayForwardR, out hitForwardData, forwardRayDistance, enemyLayerMask)) ||
            (Physics.Raycast(rayForwardL, out hitForwardData, forwardRayDistance, enemyLayerMask))))
        {
            ChangeState(BotState.SlowDown);
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

            case BotState.Stop:
                if (debug)
                {
                    print("In BotState.Stopped");
                }
                Stop();
                break;

            case BotState.SlowDown:
                if (debug)
                {
                    print("In BotState.SlowDown");
                }
                SlowDown();
                break;

            case BotState.Patrol:
                if (debug)
                {
                    print("In BotState.Patrol");
                }
                Patrol();
                break;

            case BotState.Task:
                if (debug)
                {
                    print("In BotState.Task");
                }
                Task();
                break;
            case BotState.ShutDown:
                if (debug)
                {
                    print("In BotState.ShutDown");
                }
                ShutDown();
                break;
        }
    }

    public void ChangeState(BotState state)
    {
        currentState = state;
    }

    void Idle()
    {
        agent.speed = navAgentSpeed_ORG;
        agent.radius = navAgentRadius_ORG;
        waypointIndex = 0;

        if (shutDown)
        {
            ChangeState(BotState.ShutDown);
        }
        else
        {
            ChangeState(BotState.Patrol);
        }
    }

    void Stop()
    {
        agent.speed = 0.0f;
        agent.radius = 0.1f;

        ChangeState(BotState.Idle);
    }

    void SlowDown()
    {
        agent.speed = navAgentSpeed_ORG/2;
        agent.radius = navAgentRadius_ORG/2;

        ChangeState(BotState.Idle);
    }

    void Patrol()
    {
        if (botType == BotType.NurseBot)
        {
            itemSpawnTime -= Time.deltaTime;
            if (itemSpawnTime < 0.0f)
            {
                SpawnItem();
            }
        }

        if (waypointIndex >= waypoints.Length)
        {
            ChangeState(BotState.Idle);
        }
        else
        {
            if (!this.GetComponent<TurnOffAI>().monsterHit)
            {
                agent.SetDestination(waypoints[waypointIndex].transform.position);
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    waypointIndex += 1;
                }
            }
        }

    }

    void Task()
    {
        if (botType == BotType.SecurityBot)
        {
            agent.speed = navAgentSpeed_ORG + 0.5f;
            lamp.GetComponent<MeshRenderer>().material = activeMaterial;

            if (!this.GetComponent<TurnOffAI>().monsterHit)
            {
                agent.SetDestination(objectForward.transform.position);
            }

            followWaitTime -= Time.deltaTime;
            if (followWaitTime < 0.0f)
            {
                followWaitTime = followWaitTime_ORG;
                ChangeState(BotState.Idle);
            }

        }
    }

    void ShutDown()
    {
        if (this.gameObject != null)
        {
            agent.SetDestination(startingPostion);
        }

        if (this.GetComponent<TurnOffAI>().monsterHit)
        {
            Destroy(this.gameObject);
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

    void SpawnItem()
    {
        itemToSpawn = itemTable[Random.Range(0, itemTable.Length)];

        if ((Random.value < chanceToSpawn) && (!shutDown))
        {
            GameObject spawnedObject = Instantiate(itemToSpawn, objectSpawner.transform.position, Quaternion.identity);
            spawnedObject.transform.localScale = itemScale;
        }

        itemSpawnTime = itemSpawnTime_ORG;
    }
}
