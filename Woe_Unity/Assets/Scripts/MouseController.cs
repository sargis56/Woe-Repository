using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseController : NetworkBehaviour
{
    public float sensitivity = 1000.0f;
    public Transform body;
    Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 mouse = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        //Locks the moust to the center
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        mouse.x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouse.y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotation.x -= mouse.y;

        //Stops the player from looking too far behind them
        rotation.x = Mathf.Clamp(rotation.x, -90.0f, 90.0f);

        transform.localRotation = Quaternion.Euler(rotation);
        body.Rotate(Vector3.up * mouse.x);
    }
}
