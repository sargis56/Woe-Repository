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

    public PillarObject[] pillars;

    private List<PillarObject> activePillars = new List<PillarObject>();
    private PillarObject currentPillar;
    void Start()
    {
        isComplete = false;
        for(int x = 0; x < pillars.Length; x++) {
            activePillars.Add(pillars[x]);
            pillars[x].button.buttonIndex = x;
            pillars[x].button.OnButtonClicked += CheckButton;
            pillars[x].button.GetComponent<Renderer>().material.mainTexture = pillars[x].pillar.pillarLabel;
            pillars[x].label.material.mainTexture = inActiveTexture;

        }
        AssignNextPillar();
    }

    void AssignNextPillar()
    {
        currentPillar = activePillars[UnityEngine.Random.Range(0, activePillars.Count - 1)];
        currentPillar.label.material.mainTexture = activeTexture;
    }

    void CheckButton(int index)
    {
        if (currentPillar.button.buttonIndex == index)
        {
            Debug.Log(index);
            activePillars.Remove(pillars[index]);
            if (activePillars.Count > 0)
            {
                Debug.Log(index);
                AssignNextPillar();
            }
            return;
        }
        ResetPuzzle();
    }

    void ResetPuzzle()
    {

    }
}
