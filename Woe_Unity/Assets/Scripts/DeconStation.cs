using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeconStation : MonoBehaviour
{

    public GameObject pesticideProp;
    public GameObject director;

    public bool flaskPlaced;

    // Start is called before the first frame update
    void Start()
    {
        director = GameObject.FindGameObjectWithTag("Director");
    }

    // Update is called once per frame
    void Update()
    {
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
