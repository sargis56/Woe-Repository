using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitaManager : MonoBehaviour
{
    public GameObject vitaLamp;
    public Material activeMaterial;
    public Material deactiveMaterial;

    public bool isActive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            vitaLamp.GetComponent<MeshRenderer>().material = activeMaterial;
        }
        else
        {
            vitaLamp.GetComponent<MeshRenderer>().material = deactiveMaterial;
        }
    }
}
