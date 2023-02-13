using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ButtonObject : NetworkBehaviour
{
    public delegate void ButtonClicked(int buttonIndex);
    public event ButtonClicked OnButtonClicked;

    public int buttonIndex;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnButtonClicked(buttonIndex);
        }
    }
}
