using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI itemText;

    bool hazard = false;

    public int currentHealth;
    
    public CharacterController charController;
    public Transform groundCheck;
    public float distanceFromGround = 0.4f;
    public LayerMask hazardLayerMask;
    public bool hasItem = false;
    public GameObject currentItem;
    public GameObject itemHand;
    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
    }

    // Update is called once per frame
    void Update()
    {

        healthText.text = "Health: " + currentHealth.ToString();

        if (hasItem)
        {
            itemText.text = "Item: " + currentItem.name;
            currentItem.transform.position = new Vector3(itemHand.transform.position.x, itemHand.transform.position.y, itemHand.transform.position.z);
        }
        else
        {
            itemText.text = "Item: " + "None";
        }
        

        hazard = Physics.CheckSphere(groundCheck.position, distanceFromGround, hazardLayerMask);

        if (hazard)
        {
            TakeDamage(5);
        }

        if (Input.GetButton("Fire1") && hasItem)
        {
            monster.GetComponent<MonsterController>().currentState = MonsterController.MonsternState.Retreat;
            //monster.GetComponent<MonsterController>().playerTarget = 1;
            monster.GetComponent<MonsterController>().playerTargeting = this.gameObject;
            Destroy(currentItem);
            currentItem = null;
            hasItem = false;
        }
    }

    void FixedUpdate()
    {

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

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Enemy")
        {
            TakeDamage(3);
        }
        if (hit.gameObject.tag == "Monster")
        {
            TakeDamage(5);
        }
        if (hit.gameObject.tag == "Item")
        {
            hasItem = true;
            currentItem = hit.gameObject;
            //Destroy(hit.gameObject);
        }
    }


}
