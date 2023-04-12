using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectHolder : MonoBehaviour
{
    public PlayerController playerController;

    public GameObject sprayProp;
    public GameObject noisemakerProp;
    public GameObject taserProp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.GetComponent<PlayerController>().currentItem == PlayerController.ItemState.Spray)
        {
            sprayProp.SetActive(true);
            noisemakerProp.SetActive(false);
            taserProp.SetActive(false);
        }
        else if (playerController.GetComponent<PlayerController>().currentItem == PlayerController.ItemState.Noisemaker)
        {
            sprayProp.SetActive(false);
            noisemakerProp.SetActive(true);
            taserProp.SetActive(false);
        }
        else if (playerController.GetComponent<PlayerController>().currentItem == PlayerController.ItemState.Taser)
        {
            sprayProp.SetActive(false);
            noisemakerProp.SetActive(false);
            taserProp.SetActive(true);
        }
        else
        {
            sprayProp.SetActive(false);
            noisemakerProp.SetActive(false);
            taserProp.SetActive(false);
        }
    }
}
