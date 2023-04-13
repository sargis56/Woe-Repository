using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialController : MonoBehaviour
{
    public int dialCount;
    Material defaultMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
        dialCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialCount > 9)
        {
            dialCount = 0;
        }

        if (dialCount < 0)
        {
            dialCount = 9;
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