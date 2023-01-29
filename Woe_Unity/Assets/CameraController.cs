using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour {
    public GameObject cameraHolder;
    public Vector3 offset;

    public override void OnNetworkSpawn() {
        if (!IsOwner) { return; }
        cameraHolder.SetActive(true);
    }

    public void Update() {
        cameraHolder.transform.position = transform.position + offset;
    }
}
