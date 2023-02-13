using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //private NetworkVariable<List<GameObject>> players = new NetworkVariable<List<GameObject>>(new List<GameObject>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    GameObject[] players;
    public static GameManager Instance;
    void Start()
    {
        Instance = this;
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
