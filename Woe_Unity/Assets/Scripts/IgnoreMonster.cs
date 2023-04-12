using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreMonster : MonoBehaviour
{

    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
    }

    // Update is called once per frame
    void Update()
    {
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), monster.GetComponent<CapsuleCollider>(), true);
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), monster.GetComponent<BoxCollider>(), true);
    }
}
