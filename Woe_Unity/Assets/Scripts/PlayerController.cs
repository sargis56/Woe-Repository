using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI ammoText;

    public enum ItemState {Empty, Spray, Noisemaker, Taser};
    public ItemState currentItem;
    int sprayAmmo = 0;
    int noisemakerAmmo = 0;
    int taserAmmo = 0;

    //public GameObject currentItem;

    bool hazard = false;

    public int currentHealth;
    
    public CharacterController charController;
    public Transform groundCheck;
    public float distanceFromGround = 0.4f;
    public LayerMask hazardLayerMask;
    
    public bool hasSpray = false;
    public bool hasNoisemaker = false;
    public bool hasTaser = false;
    public bool hasInjector = false;

    public GameObject itemHand;
    public GameObject monster;

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

    public GameObject pauseMenu;
    public bool isInMenu;
    //public CameraController cameraController;

    //void Awake()
    //{
    //    pauseMenu = GameObject.Find("PauseMenu");
    //}
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        //pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        currentItem = ItemState.Empty;
        monster = GameObject.FindGameObjectWithTag("Monster");
        isInMenu = false;
        //healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
        //ammoText = GameObject.FindGameObjectWithTag("StaminaText").GetComponent<TextMeshProUGUI>();
        //itemText = GameObject.FindGameObjectWithTag("ItemText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        //healthText.text = "Health: " + currentHealth.ToString();

        SetupRays();
        UpdateStates();

        //if (hasSpray)
        //{
        //    itemText.text = "Item: " + currentItem.ToString();
        //    currentItem.transform.position = new Vector3(itemHand.transform.position.x, itemHand.transform.position.y, itemHand.transform.position.z);
        //}
        //else
        //{
        //    itemText.text = "Item: " + "None";
        //}

        hazard = Physics.CheckSphere(groundCheck.position, distanceFromGround, hazardLayerMask);

        if (hazard)
        {
            TakeDamage(1);
        }

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown("[8]") && debug)
        {
            TakeDamage(50);
        }

        if (Input.GetKeyDown("[9]") && debug)
        {
            TakeDamage(100);
        }

        if (isInMenu)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //cameraController.cameraMovementToggle = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isInMenu = true;
            if (pauseMenu == null) { return; }
            pauseMenu.SetActive(true);
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
        //if (Input.GetKeyDown("4") && hasInjector)
        //{
        //    ChangeItem(ItemState.Injector);
        //}

        if (Input.GetKeyDown("e") && requestHealth)
        {
            AddHealth(5);
        }

        requestHealth = false;
        if (itemText == null) { return; }
        itemText.text = "Item: " + currentItem.ToString();
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
        if (ammoText == null) { return; }
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
        currentHealth -= damage;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log(hit.gameObject.name);
        //if (hit.gameObject.tag == "Enemy")
        //{
        //    TakeDamage(3);
        //}
        //if (hit.gameObject.tag == "Monster")
        //{
        //    TakeDamage(1);
        //}
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
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Monster")
    //    {
    //        TakeDamage(1);
    //    }
    //}


}
