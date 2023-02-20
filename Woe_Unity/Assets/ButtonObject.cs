using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ButtonObject : NetworkBehaviour
{
    public delegate void ButtonClicked(int buttonIndex);
    public event ButtonClicked OnButtonClicked;

    private bool canTrigger = false;
    public int buttonIndex;
    [HideInInspector] public GameObject interactiveTextPrompt;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactiveTextPrompt.gameObject.SetActive(true);
            canTrigger = true;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        if (Input.GetButton("Interact"))
    //        {
    //            OnButtonClicked(buttonIndex);
    //        }
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactiveTextPrompt.gameObject.SetActive(false);
            canTrigger = false;
        }
    }

    private void Update()
    {
        if (canTrigger && Input.GetButton("Interact"))
        {
            OnButtonClicked(buttonIndex);
            canTrigger = false;
        }
    }
}
