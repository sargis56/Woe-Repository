using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log(OwnerClientId + " - " + randomNumber.Value);
        };
    }

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Update is called once per frame
    void Update() {
        if (!IsOwner) { return; }

        if (Input.GetKeyDown(KeyCode.T)) {
            randomNumber.Value = Random.Range(0, 10);
        }

        Vector3 moveDirection = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W)) {
            moveDirection.z = +1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveDirection.z = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveDirection.x = +1f;
        }
        float moveSpeed = 10.0f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
