using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI livesText;

    [SerializeField]
    private Transform respawnPosition;

    public enum PlayerState { Alive, Dead };
    public PlayerState playerState;

    public enum ItemState {Empty, Spray, Noisemaker, Taser};
    public ItemState currentItem;
    int sprayAmmo = 0;
    int noisemakerAmmo = 0;
    int taserAmmo = 0;

    //public GameObject currentItem;

    bool hazard = false;
    bool enemyHit = false;
    
    public int currentHealth;
    
    public CharacterController charController;
    public Transform groundCheck;
    public float distanceFromGround = 0.4f;
    public LayerMask hazardLayerMask;

    [SerializeField]
    private bool hasSpray = false;
    [SerializeField]
    private bool hasNoisemaker = false;
    [SerializeField]
    private bool hasTaser = false;

    public GameObject itemHand;
    public GameObject monster;
    public GameObject director;

    bool monsterInRange = false;
    bool botInRange = false;
    bool enemyInRange = false;

    public LayerMask monsterLayerMask;
    public LayerMask botLayerMask;
    public LayerMask enemyLayerMask;

    public float forwardRayDistance = 7.5f;
    public float forwardRayHeight = 0.0f;

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

    public bool requestHealth = false;

    public float pressure = 0.0f;

    public bool debug = false;

    [SerializeField]
    private float damageProtectTime = 3.0f;
    float damageProtectTime_ORG;
    [SerializeField]
    private bool canTakeDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        director = GameObject.FindGameObjectWithTag("Director");

        damageProtectTime_ORG = damageProtectTime;
        playerState = PlayerState.Alive;
        currentItem = ItemState.Empty;
        monster = GameObject.FindGameObjectWithTag("Monster");
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
        ammoText = GameObject.FindGameObjectWithTag("StaminaText").GetComponent<TextMeshProUGUI>();
        itemText = GameObject.FindGameObjectWithTag("ItemText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        livesText.text = "Lives: " + director.GetComponent<GameController>().playerLives.ToString();

        switch (playerState)
        {
            case PlayerState.Alive:
                if (debug)
                {
                    print("Player is alive");
                }
                UpdateAlive();
                break;

            case PlayerState.Dead:
                if (debug)
                {
                    print("Player is dead");
                }
                UpdateDead();
                break;
        }
    }

    void UpdateDead()
    {
        healthText.text = "Dead";

        charController.enabled = false;
        transform.position = new Vector3(respawnPosition.position.x, respawnPosition.position.y, respawnPosition.position.z);
        transform.rotation = Quaternion.identity;
        charController.enabled = true;

        if (director.GetComponent<GameController>().deadPlayersNum > 0)
        {
            director.GetComponent<GameController>().TakeDeadPlayer(1);
        }
        AddHealth(999);
        ChangeState(PlayerState.Alive);

    }

    void UpdateAlive()
    {
        healthText.text = "Health: " + currentHealth.ToString();

        respawnPosition = director.GetComponent<GameController>().GetClosestVitaChamber(this.gameObject.transform.position).transform;

        SetupRays();
        UpdateStates();

        hazard = Physics.CheckSphere(groundCheck.position, distanceFromGround, hazardLayerMask);
        enemyHit = Physics.CheckSphere(groundCheck.position, distanceFromGround, enemyLayerMask);

        if (hazard)
        {
            TakeDamage(5);
        }
        if (enemyHit)
        {
            TakeDamage(15);
        }

        if (currentHealth <= 0)
        {
            ChangeState(PlayerState.Dead);
            director.GetComponent<GameController>().TakeLives(1);
            director.GetComponent<GameController>().AddDeadPlayer(1);
        }

        if (Input.GetKeyDown(",") && debug)
        {
            TakeDamage(50);
        }

        if (Input.GetKeyDown(".") && debug)
        {
            AddHealth(50);
        }

        if (Input.GetKeyDown("0"))
        {
            ChangeItem(ItemState.Empty);
        }
        if (Input.GetKeyDown("1") && hasSpray)
        {
            ChangeItem(ItemState.Spray);
        }
        if (Input.GetKeyDown("2") && hasNoisemaker)
        {
            ChangeItem(ItemState.Noisemaker);
        }
        if (Input.GetKeyDown("3") && hasTaser)
        {
            ChangeItem(ItemState.Taser);
        }

        if (Input.GetKeyDown("e") && requestHealth)
        {
            AddHealth(5);
        }
        requestHealth = false;

        if (currentItem == ItemState.Empty)
        {
            itemText.text = "";
        }
        else
        {
            itemText.text = "Item: " + currentItem.ToString();
        }

        if (canTakeDamage != true)
        {
            damageProtectTime -= Time.deltaTime;
            if (damageProtectTime < 0.0f)
            {
                canTakeDamage = true;
                damageProtectTime = damageProtectTime_ORG;
            }
        }
    }

    void SetupRays()
    {
        //Forward ray setup
        forwardMAngle = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM = forwardMAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardM = new Ray(transform.position, forwardM);

        Debug.DrawRay(transform.position, forwardM * forwardRayDistance, Color.white);

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, monsterLayerMask))
        {
            monsterInRange = true;
            objectForward = hitForwardData.collider.gameObject;
            if ((currentItem == ItemState.Spray) && (monster.GetComponent<MonsterController>().currentState != MonsterController.MonsterState.Retreat))
            {
                objectForward.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Caution;
            }
            
        }
        else
        {
            monsterInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, botLayerMask))
        {
            botInRange = true;
            objectForward = hitForwardData.collider.gameObject;
        }
        else
        {
            botInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, enemyLayerMask))
        {
            enemyInRange = true;
            objectForward = hitForwardData.collider.gameObject;
        }
        else
        {
            enemyInRange = false;
        }

        Debug.DrawRay(transform.position, forwardM * hitForwardData.distance, Color.yellow);
    }

    void FixedUpdate()
    {

    }

    void UpdateStates()
    {
        switch (currentItem)
        {
            case ItemState.Empty:
                ammoText.text = "";
                break;

            case ItemState.Spray:
                ammoText.text = "Ammo: " + sprayAmmo.ToString();


                if ((Input.GetButton("Fire1")) && (sprayAmmo > 0))
                {
                    if (monsterInRange)
                    {
                        monster.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Retreat;
                        monster.GetComponent<MonsterController>().playerTargeting = this.gameObject;
                    }
                    if (enemyInRange)
                    {
                        Destroy(objectForward.gameObject);
                    }
                    sprayAmmo -= 1;
                }
                break;

            case ItemState.Noisemaker:
                ammoText.text = "Ammo: " + noisemakerAmmo.ToString();

                if ((Input.GetButton("Fire1")) && (noisemakerAmmo > 0))
                {

                    noisemakerAmmo -= 1;
                }
                break;

            case ItemState.Taser:
                ammoText.text = "Ammo: " + taserAmmo.ToString();

                if ((Input.GetButton("Fire1")) && (taserAmmo > 0))
                {
                    if (botInRange)
                    {
                        Destroy(objectForward.gameObject);
                    }
                    taserAmmo -= 1;
                }
                break;

            //case ItemState.Injector:
            //    ammoText.text = "";

            //    break;

        }
    }

    public void ChangeItem(ItemState state)
    {
        currentItem = state;
    }

    public void AddHealth(int health_)
    {
        currentHealth = currentHealth + health_;
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            currentHealth -= damage;
            canTakeDamage = false;
        }
    }

    public void TakeTrueDamage(int damage)
    {
        currentHealth -= damage;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Spray")
        {
            if (hasSpray == false)
            {
                ChangeItem(ItemState.Spray);
                hasSpray = true;
            }
            sprayAmmo += 1;
            Destroy(hit.gameObject);
        }
        if (hit.gameObject.tag == "Noisemaker")
        {
            if (hasNoisemaker == false)
            {
                ChangeItem(ItemState.Noisemaker);
                hasNoisemaker = true;
            }
            noisemakerAmmo += 1;
            Destroy(hit.gameObject);
        }
        if (hit.gameObject.tag == "Taser")
        {
            if (hasTaser == false)
            {
                ChangeItem(ItemState.Taser);
                hasTaser = true;
            }
            taserAmmo += 1;
            Destroy(hit.gameObject);
        }
        if (hit.gameObject.tag == "Heart")
        {
            director.GetComponent<GameController>().AddLives(1);
            Destroy(hit.gameObject);
        }
    }

    public void ChangeState(PlayerState state)
    {
        playerState = state;
    }


}
