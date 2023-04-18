using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DialController : NetworkBehaviour
{
    public int dialCount;
    Material defaultMaterial;

    public int dialCountMax = 9;
    public int dialCountMin = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
        dialCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        if (dialCount > dialCountMax)
        {
            dialCount = dialCountMin;
        }

        if (dialCount < dialCountMin)
        {
            dialCount = dialCountMax;
        }

        this.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public void DialUp()
    {
        dialCount = dialCount + 1;
    }

    public void DialDown()
    {
        dialCount = dialCount - 1;
    }
}