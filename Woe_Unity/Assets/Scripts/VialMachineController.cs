using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class VialMachineController : NetworkBehaviour
{
    public GameObject dial1;
    public GameObject dial2;
    public GameObject dial3;

    public TextMeshProUGUI dial1Text;
    public TextMeshProUGUI dial2Text;
    public TextMeshProUGUI dial3Text;

    public GameObject pesticide;

    [SerializeField]
    private int formulaMin;
    [SerializeField]
    private int formulaMax;

    public int comp1;
    public int comp2;
    public int comp3;

    [SerializeField]
    private int selectedComp1;
    [SerializeField]
    private int selectedComp2;
    [SerializeField]
    private int selectedComp3;

    [SerializeField]
    private NetworkVariable<bool> solved = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    public NetworkVariable<bool> comp1Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp2Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp3Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp4Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp5Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp6Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp7Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp8Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> comp9Found = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    public bool lastInput = true;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        comp1 = Random.Range(formulaMin, (formulaMax + 1));
        comp2 = Random.Range(formulaMin, (formulaMax + 1));
        comp3 = Random.Range(formulaMin, (formulaMax + 1));

        pesticide = GameObject.FindGameObjectWithTag("Pesticide");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned) { return; }
        if ((comp1 == comp2) || (comp1 == comp3))
        {
            comp1 = Random.Range(formulaMin, (formulaMax + 1));
        }

        if ((comp2 == comp1) || (comp2 == comp3))
        {
            comp2 = Random.Range(formulaMin, (formulaMax + 1));
        }

        if ((comp3 == comp1) || (comp3 == comp2))
        {
            comp3 = Random.Range(formulaMin, (formulaMax + 1));
        }

        selectedComp1 = dial1.GetComponent<DialController>().dialCount;
        selectedComp2 = dial2.GetComponent<DialController>().dialCount;
        selectedComp3 = dial3.GetComponent<DialController>().dialCount;

        if (lastInput)
        {
            CheckDials(false);
        }
        else
        {
            CheckDials(true);
        }

        if ((selectedComp1 == comp1) && (selectedComp2 == comp2) && (selectedComp3 == comp3))
        {
            solved.Value = true;
        }

        if (solved.Value)
        {
            dial1Text.text = "Solved";
            dial2Text.text = "Solved";
            dial3Text.text = "Solved";
            if (pesticide != null)
            {
                pesticide.gameObject.SetActive(true);
            }
        }
        else
        {
            dial1Text.text = "Compound: " + selectedComp1.ToString();
            dial2Text.text = "Compound: " + selectedComp2.ToString();
            dial3Text.text = "Compound: " + selectedComp3.ToString();
            pesticide.gameObject.SetActive(false);
        }
    }

    void CheckDials(bool dir)
    {
        if (comp1Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 1)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 1)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 1)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp2Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 2)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 2)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 2)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp3Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 3)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 3)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 3)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp4Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 4)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 4)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 4)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp5Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 5)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 5)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 5)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp6Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 6)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 6)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 6)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp7Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 7)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 7)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 7)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp8Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 8)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 8)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 8)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
        if (comp9Found.Value == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 9)
            {
                if (dir)
                {

                    dial1.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial1.GetComponent<DialController>().DialDown();
                }
            }
            if (dial2.GetComponent<DialController>().dialCount == 9)
            {
                if (dir)
                {

                    dial2.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial2.GetComponent<DialController>().DialDown();
                }
            }
            if (dial3.GetComponent<DialController>().dialCount == 9)
            {
                if (dir)
                {

                    dial3.GetComponent<DialController>().DialUp();
                }
                else
                {
                    dial3.GetComponent<DialController>().DialDown();
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp1ServerRpc()
    {
        comp1Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp2ServerRpc()
    {
        comp2Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp3ServerRpc()
    {
        comp3Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp4ServerRpc()
    {
        comp4Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp5ServerRpc()
    {
        comp5Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp6ServerRpc()
    {
        comp6Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp7ServerRpc()
    {
        comp7Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp8ServerRpc()
    {
        comp8Found.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void FoundComp9ServerRpc()
    {
        comp9Found.Value = true;
    }
}
