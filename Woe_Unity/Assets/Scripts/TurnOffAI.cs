using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurnOffAI : MonoBehaviour
{
    private bool monsterHit = false;
    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(monster.transform.position, this.transform.position) > 5.0f)
        {
            monsterHit = false;
        }
        else
        {
            monsterHit = true;
        }

        if (monsterHit)
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
