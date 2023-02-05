using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeMap : MonoBehaviour
{
    private class PillarObject
    {
        public PillarObject(PillarSpawner pillar, ButtonObject button_, MeshRenderer label_)
        {
            //texture = pillar.pillarLabel;
            button = button_;
            button.GetComponent<MeshRenderer>().material.mainTexture = pillar.pillarLabel;
            label = label_;
        }
        //public Texture texture;
        public ButtonObject button;
        public MeshRenderer label;
    }

    public bool isComplete;
    public Texture inActiveTexture;
    public Texture activeTexture;

    public PillarSpawner[] pillarSpawners;
    public ButtonObject[] pillarButtons;
    public MeshRenderer[] pillarLabels;

    private List<PillarObject> activePillars = new List<PillarObject>();
    private PillarObject currentPillar;
    void Start()
    {
        isComplete = false;
        for(int x = 0; x < pillarSpawners.Length; x++) {
            activePillars.Add(new PillarObject(pillarSpawners[x], pillarButtons[x], pillarLabels[x]));
            pillarButtons[x].buttonIndex = x;
            pillarButtons[x].OnButtonClicked += CheckButton;
            pillarLabels[x].material.mainTexture = inActiveTexture;
        }
        AssignNextPillar();
    }

    void AssignNextPillar()
    {
        currentPillar = activePillars[Random.Range(0, activePillars.Count)];
        currentPillar.label.material.mainTexture = activeTexture;
    }

    void CheckButton(int index)
    {
        if (currentPillar.button.buttonIndex == index)
        {
            activePillars.Remove(activePillars[index]);
            AssignNextPillar();
            return;
        }
        ResetPuzzle();
    }

    void ResetPuzzle()
    {

    }
}
