using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //private NetworkVariable<List<GameObject>> players = new NetworkVariable<List<GameObject>>(new List<GameObject>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public GameObject[] players;
    public static GameManager Instance;
    private ButtonObject[] buttonObjects;
    public GameObject interactiveTextPrompt;

    void Awake()
    {
        Instance = this;

        GameObject[] buttonGameObjects = GameObject.FindGameObjectsWithTag("Button");
        buttonObjects = new ButtonObject[buttonGameObjects.Length];
        for (int i = 0; i < buttonGameObjects.Length; i++)
        {
            buttonObjects[i] = buttonGameObjects[i].GetComponent<ButtonObject>();
            buttonObjects[i].interactiveTextPrompt = interactiveTextPrompt;
        }
    }

    public void AddPlayer(GameObject player)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        //foreach(GameObject player in players)
        //{
        //    Debug.Log(player.transform.localPosition.x);
        //}
    }
}
