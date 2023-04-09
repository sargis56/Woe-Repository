using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VialMachineController : MonoBehaviour
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
    private bool solved;

    public bool comp1Found = false;
    public bool comp2Found = false;
    public bool comp3Found = false;
    public bool comp4Found = false;
    public bool comp5Found = false;
    public bool comp6Found = false;
    public bool comp7Found = false;
    public bool comp8Found = false;
    public bool comp9Found = false;

    // Start is called before the first frame update
    void Start()
    {
        solved = false;
        comp1 = Random.Range(formulaMin, (formulaMax + 1));
        comp2 = Random.Range(formulaMin, (formulaMax + 1));
        comp3 = Random.Range(formulaMin, (formulaMax + 1));

        pesticide = GameObject.FindGameObjectWithTag("Pesticide");
    }

    // Update is called once per frame
    void Update()
    {

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

        CheckDials();

        if ((selectedComp1 == comp1) && (selectedComp2 == comp2) && (selectedComp3 == comp3))
        {
            solved = true;
        }

        if (solved)
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

    void CheckDials()
    {
        if (comp1Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 1)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 1)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 1)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp2Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 2)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 2)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 2)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp3Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 3)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 3)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 3)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp4Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 4)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 4)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 4)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp5Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 5)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 5)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 5)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp6Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 6)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 6)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 6)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp7Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 7)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 7)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 7)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp8Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 8)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 8)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 8)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
        if (comp2Found == false)
        {
            if (dial1.GetComponent<DialController>().dialCount == 9)
            {

                dial1.GetComponent<DialController>().DialUp();
            }
            if (dial2.GetComponent<DialController>().dialCount == 9)
            {

                dial2.GetComponent<DialController>().DialUp();
            }
            if (dial3.GetComponent<DialController>().dialCount == 9)
            {

                dial3.GetComponent<DialController>().DialUp();
            }
        }
    }
}
