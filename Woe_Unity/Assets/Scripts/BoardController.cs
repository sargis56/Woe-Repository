using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class BoardController : NetworkBehaviour
{
    [SerializeField]
    private int compMin;
    [SerializeField]
    private int compMax;

    public GameObject machine;

    public TextMeshProUGUI[] compText;

    int compIndex;

    [SerializeField]
    private int[] compArray = new int[3];
    public int compArrayIndex;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        compIndex = Random.Range(0, compText.Length);
        compArray = new int[3];

        foreach (TextMeshProUGUI text in compText)
        {
            text.text = "Compound: " + Random.Range(compMin, (compMax + 1)).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        compArray[0] = machine.GetComponent<VialMachineController>().comp1;
        compArray[1] = machine.GetComponent<VialMachineController>().comp2;
        compArray[2] = machine.GetComponent<VialMachineController>().comp3;

        compText[compIndex].text = "Compound: " + compArray[compArrayIndex].ToString();
        compText[compIndex].fontStyle = FontStyles.Normal;
        compText[compIndex].fontStyle = FontStyles.Italic;

        foreach (TextMeshProUGUI text in compText)
        {
            if ( (text.text == "Compound: " +  compArray[0]) || (text.text == "Compound: " + compArray[1]) || (text.text == "Compound: " + compArray[2]) )
            {
                if (text != compText[compIndex])
                {
                    text.text = "Compound: " + Random.Range(compMin, (compMax + 1)).ToString();
                }
            }
        }

    }
}
