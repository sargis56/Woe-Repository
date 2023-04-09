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
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI livesText;
    public Material selectMaterial;

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
    public bool hasPestFlask = false;

    public GameObject itemHand;
    public GameObject monster;
    public GameObject director;
    public GameObject pesticideMachine;
    public GameObject deconStation;

    public bool monsterInRange = false;
    public bool botInRange = false;
    public bool enemyInRange = false;
    public bool buttonInRange = false;
    public bool deconButtonInRange = false;

    public LayerMask monsterLayerMask;
    public LayerMask botLayerMask;
    public LayerMask enemyLayerMask;

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
        infoText = GameObject.FindGameObjectWithTag("InfoText").GetComponent<TextMeshProUGUI>();

        pesticideMachine = GameObject.FindGameObjectWithTag("PesticideMachine");
        deconStation = GameObject.FindGameObjectWithTag("DeconStation");
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

        if ((botInRange == false) && (buttonInRange == false) &&  (deconButtonInRange == false))
        {
            infoText.text = "";
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

        if (Input.GetKeyDown("e") && buttonInRange) 
        {
            objectForward.GetComponent<DialController>().DialUp();
        }
        if (Input.GetKeyDown("q") && buttonInRange)
        {
            objectForward.GetComponent<DialController>().DialDown();
        }
        if (Input.GetKeyDown("e") && deconButtonInRange) 
        {
            if (deconStation.GetComponent<DeconStation>().flaskPlaced)
            {
                director.GetComponent<GameController>().decontamination = true;
            }
        }

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

        if (hit.gameObject.tag == "C1")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp1Found = true;
        }
        if (hit.gameObject.tag == "C2")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp2Found = true;
        }
        if (hit.gameObject.tag == "C3")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp3Found = true;
        }
        if (hit.gameObject.tag == "C4")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp4Found = true;
        }
        if (hit.gameObject.tag == "C5")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp5Found = true;
        }
        if (hit.gameObject.tag == "C6")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp6Found = true;
        }
        if (hit.gameObject.tag == "C7")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp7Found = true;
        }
        if (hit.gameObject.tag == "C8")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp8Found = true;
        }
        if (hit.gameObject.tag == "C9")
        {
            Destroy(hit.gameObject);
            pesticideMachine.GetComponent<VialMachineController>().comp9Found = true;
        }
        if (hit.gameObject.tag == "Pesticide")
        {
            Destroy(hit.gameObject);
            hasPestFlask = true;
        }
    }

    public void ChangeState(PlayerState state)
    {
        playerState = state;
    }


}
