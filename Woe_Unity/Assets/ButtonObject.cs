using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ButtonObject : NetworkBehaviour
{
    public delegate void ButtonClicked();
    public event ButtonClicked OnButtonClicked;

    private void OnTriggerEnter(Collider other)
    {
        OnButtonClicked();
    }
}
