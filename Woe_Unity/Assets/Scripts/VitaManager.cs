using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VitaManager : NetworkBehaviour
{
    public GameObject vitaLamp;
    public Material vitaLampMat;
    public Material activeMaterial;
    public Material deactiveMaterial;

    //public bool isActive = true;
    [SerializeField]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        if (isActive.Value)
        {
            vitaLampMat = activeMaterial;
        }
        else
        {
            vitaLampMat = deactiveMaterial;
        }

        vitaLamp.GetComponent<MeshRenderer>().material = vitaLampMat;
    }
}
