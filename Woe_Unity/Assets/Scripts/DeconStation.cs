using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeconStation : NetworkBehaviour
{

    public GameObject pesticideProp;
    public GameObject director;

    public bool flaskPlaced;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        director = GameObject.FindGameObjectWithTag("Director");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        if (flaskPlaced)
        {
            pesticideProp.gameObject.SetActive(true);
        }
        else
        {
            pesticideProp.gameObject.SetActive(false);
        }

    }
}
