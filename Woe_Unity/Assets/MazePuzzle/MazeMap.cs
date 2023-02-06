using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeMap : MonoBehaviour
{
    [Serializable]
    public class PillarObject
    {
        //public PillarObject(PillarSpawner pillar, ButtonObject button_, MeshRenderer label_)
        //{
        //    //texture = pillar.pillarLabel;
        //    button = button_;
        //    button.GetComponent<MeshRenderer>().material.mainTexture = pillar.pillarLabel;
        //    label = label_;
        //}
        public PillarSpawner pillar;
        public ButtonObject button;
        public MeshRenderer label;
    }

    public bool isComplete;
    public Texture inActiveTexture;
    public Texture activeTexture;
    public DoorObject[] exitDoors;

    public PillarObject[] pillars;

    private List<PillarObject> activePillars = new List<PillarObject>();
    private PillarObject currentPillar;
    void Start()
    {
        for (int x = 0; x < pillars.Length; x++)
        {
            pillars[x].button.buttonIndex = x;
            pillars[x].button.OnButtonClicked += CheckButton;
            pillars[x].button.GetComponent<Renderer>().material.mainTexture = pillars[x].pillar.pillarLabel;
        }
        ResetPuzzle();
    }

    void AssignNextPillar()
    {
        currentPillar = activePillars[UnityEngine.Random.Range(0, activePillars.Count - 1)];
        currentPillar.label.material.mainTexture = activeTexture;
    }

    void CheckButton(int index)
    {
        if (isComplete)
        {
            return;
        }

        Debug.Log("trigger");
        if (currentPillar.button.buttonIndex == index)
        {
            activePillars.Remove(currentPillar);
            if (activePillars.Count > 0)
            {
                AssignNextPillar();
            }
            else
            {
                isComplete = true;
                foreach(DoorObject door in exitDoors)
                {
                    door.ToggleDoor(0);
                }
            }
            return;
        }
        ResetPuzzle();
    }

    void ResetPuzzle()
    {
        Debug.Log("reset");
        activePillars.Clear();
        isComplete = false;
        for (int x = 0; x < pillars.Length; x++)
        {
            activePillars.Add(pillars[x]);
            pillars[x].label.material.mainTexture = inActiveTexture;
        }
        AssignNextPillar();
    }
}
