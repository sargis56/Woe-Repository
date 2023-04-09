using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarSpawner : MonoBehaviour
{
    public GameObject pillarPrehab;
    public Texture pillarLabel;
    // Start is called before the first frame update
    void Awake()
    {
        //int iterator = 0;
        GameObject current = Instantiate(pillarPrehab, transform.position, transform.rotation, this.transform);
        current.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = pillarLabel;
        current.transform.GetChild(1).GetComponent<MeshRenderer>().material.mainTexture = pillarLabel;
        current.transform.GetChild(2).GetComponent<MeshRenderer>().material.mainTexture = pillarLabel;
        current.transform.GetChild(3).GetComponent<MeshRenderer>().material.mainTexture = pillarLabel;
        //iterator++;
    }
}
