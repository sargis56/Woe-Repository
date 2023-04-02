using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour {
    public GameObject cameraHolder;
    public Vector3 offset;

    public Transform crouchPoint;
    public Transform straightPoint;

    //public bool cameraMovementToggle;

    public override void OnNetworkSpawn() {
        if (!IsOwner) { return; }
        //cameraMovementToggle = true;
        cameraHolder.SetActive(true);
    }

    public void Update() {
        //if (cameraMovementToggle)
        //{
            cameraHolder.transform.position = transform.position + offset;
        //}
    }
}
