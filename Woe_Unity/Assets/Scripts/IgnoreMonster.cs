using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreMonster : MonoBehaviour
{

    public GameObject monster;
    public bool isBoxCollider = true;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Director").GetComponent<GameController>().monster;
    }

    // Update is called once per frame
    void Update()
    {
        if (monster != null)
        {
            if (isBoxCollider)
            {
                Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), monster.GetComponent<CapsuleCollider>(), true);
            }
            else
            {
                Physics.IgnoreCollision(this.GetComponent<CapsuleCollider>(), monster.GetComponent<CapsuleCollider>(), true);
            }
        }

    }
}
