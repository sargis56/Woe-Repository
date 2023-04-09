using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReset : MonoBehaviour
{
    Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}
