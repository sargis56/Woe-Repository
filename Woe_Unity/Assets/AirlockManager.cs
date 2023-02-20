using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockManager : MonoBehaviour
{
    public AirlockObject[] airlocks;
    bool airlockState = false;

    public void ToggleAirlocks()
    {
        airlockState = !airlockState;
        if (airlockState == true)
        {
            for (int i = 0; i < airlocks.Length; i++)
            {
                airlocks[i].exitDoor.ToggleDoor(0);
                airlocks[i].isActive = false;
            }
            return;
        }
        for (int i = 0; i < airlocks.Length; i++)
        {
            airlocks[i].enterDoor.ToggleDoor(0);
            airlocks[i].isActive = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < airlocks.Length; i++)
        {
            if (!airlocks[i].isActive)
            {
                return;
            }
        }

        ToggleAirlocks();
    }
}
