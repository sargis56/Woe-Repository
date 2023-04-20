using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RemoveObject : NetworkBehaviour
{
    [SerializeField]
    private float lingerTime = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lingerTime -= Time.deltaTime;
        if (lingerTime < 0.0f)
        {
            //Destroy(this.gameObject);
            NetworkManager.Destroy(this.gameObject);
        }
    }
}
