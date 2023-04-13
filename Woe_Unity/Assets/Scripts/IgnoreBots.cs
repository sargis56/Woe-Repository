using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreBots : MonoBehaviour
{
    public GameObject[] bots;
    public bool isBoxCollider = true;

    // Start is called before the first frame update
    void Start()
    {
        bots = GameObject.FindGameObjectsWithTag("Bot");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject bot in bots)
        {
            if (bot != null)
            {
                if (isBoxCollider)
                {
                    Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), bot.GetComponent<BoxCollider>(), true);
                }
                else
                {
                    Physics.IgnoreCollision(this.GetComponent<CapsuleCollider>(), bot.GetComponent<BoxCollider>(), true);
                }    
            }
        }
    }
}
