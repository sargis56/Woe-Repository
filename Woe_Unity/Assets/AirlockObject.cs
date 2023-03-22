using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockObject : MonoBehaviour
{
    public bool isActive = false;
    public DoorObject enterDoor;
    public DoorObject exitDoor;

    private bool airlockState = false;

    void Start()
    {
        isActive = false;
        enterDoor.ToggleDoor(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isActive = true;
            airlockState = !airlockState;
            if (airlockState == true)
            {
                enterDoor.ToggleDoor(0);
                return;
            }
            exitDoor.ToggleDoor(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isActive = false;
        }
    }
}
