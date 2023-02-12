using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    bool hazard = false;

    public int currentHealth;
    
    public CharacterController charController;
    public Transform groundCheck;
    public float distanceFromGround = 0.4f;
    public LayerMask hazardLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        hazard = Physics.CheckSphere(groundCheck.position, distanceFromGround, hazardLayerMask);

        if (hazard)
        {
            TakeDamage(5);
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
    }
}
