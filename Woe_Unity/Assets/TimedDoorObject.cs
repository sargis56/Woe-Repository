using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TimedDoorObject : MonoBehaviour
{
    public ButtonObject linkedButton;
    public bool MoveOnX;
    public bool MoveOnY;
    public bool MoveOnZ;
    public float speed = 1;
    public float distance;

    private Vector3 originalPosition;
    private bool isOpening = false;
    private bool isMoving = false;

    public float timeOpen = 10;
    private float timer = 0.0f;
    private bool paused;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        paused = false;
        originalPosition = transform.localPosition;
        if (linkedButton != null)
        {
            linkedButton.OnButtonClicked += ToggleDoor;
        }
    }

    public void ToggleDoor(int buttonIndex)
    {
        if (isMoving == true)
        {
            return;
        }
        isOpening = !isOpening;
        isMoving = true;
        paused = false;
        timer = 0.0f;
    }

    void MoveDoor()
    {
        bool locationUpdated = false;
        float newX = transform.localPosition.x;
        float newY = transform.localPosition.y;
        float newZ = transform.localPosition.z;

        switch (isOpening)
        {
            case true:
                if (MoveOnX == true)
                {
                    if (transform.localPosition.x <= distance)
                    {
                        newX += speed;
                        locationUpdated = true;
                    }
                }
                if (MoveOnY == true)
                {
                    if (transform.localPosition.y <= distance)
                    {
                        newY += speed;
                        locationUpdated = true;
                    }
                }
                if (MoveOnZ == true)
                {
                    if (transform.localPosition.z <= distance)
                    {
                        newZ += speed;
                        locationUpdated = true;
                    }
                }
                break;
            case false:
                if (MoveOnX == true)
                {
                    if (transform.localPosition.x >= originalPosition.x)
                    {
                        newX -= speed;
                        locationUpdated = true;
                    }
                }
                if (MoveOnY == true)
                {
                    if (transform.localPosition.y >= originalPosition.y)
                    {
                        newY -= speed;
                        locationUpdated = true;
                    }
                }
                if (MoveOnZ == true)
                {
                    if (transform.localPosition.z >= originalPosition.z)
                    {
                        newZ -= speed;
                        locationUpdated = true;
                    }
                }
                break;
        }

        if (locationUpdated == true)
        {
            transform.localPosition = new Vector3(newX, newY, newZ);
            return;
        }

        if (paused == false)
        {
            paused = true;
            return;
        }

        if (paused)
        {
            if (timer < timeOpen)
            {
                return;
            }
            if (isOpening == true)
            {
                isOpening = false;
                return;
            }
        }

        isMoving = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving == true)
        {
            MoveDoor();
            timer += Time.deltaTime;
        }
    }
}
