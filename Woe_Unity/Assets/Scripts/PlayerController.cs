using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentHealth;
    
    public CharacterController charController;
    public Transform groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            //Do something when ded
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
