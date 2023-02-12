using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    public bool Debug = false;

    bool grounded = false;
    bool bounce = false;
    Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
    public float velRest = -2.0f;
    public float speed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;
    public float bounceHeight = 1.0f;
    public float bounceHeight_ORG;

    public CharacterController charController;
    public GameObject camera;
    public Transform groundCheck;
    public Transform spawnPoint;
    public Transform crouchPoint;
    public Transform straightPoint;
    public float distanceFromGround = 0.4f;
    public LayerMask groundLayerMask;
    public LayerMask bounceLayerMask;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        bounceHeight_ORG = bounceHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        grounded = Physics.CheckSphere(groundCheck.position, distanceFromGround, groundLayerMask);
        bounce = Physics.CheckSphere(groundCheck.position, distanceFromGround, bounceLayerMask);

        if (grounded && vel.y < 0.0f)
        {
            if (Debug)
            {
                print("Player: Grounded");
            }
            vel.y = velRest;
        }

        if (bounce && vel.y < 0.0f)
        {
            if (Debug)
            {
                print("Player: Bounce");
            }
            Jump(bounceHeight);
        }
        bounceHeight = bounceHeight_ORG;

        camera.transform.position = new Vector3(straightPoint.position.x, straightPoint.position.y, straightPoint.position.z);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetButton("Crouch"))
        {
            if (Debug)
            {
                print("Player: Crouch");
            }
            
            Crouch();

        }

        if (Input.GetButton("Sprint"))
        {
            if (Debug)
            {
                print("Player: Sprint");
            }
            
            Move(moveHorizontal, moveVertical, sprintSpeed);
        }
        else
        {
            Move(moveHorizontal, moveVertical, speed);
        }

        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump(jumpHeight);
        }

        vel.y += gravity * Time.deltaTime;
        charController.Move(vel * Time.deltaTime);
    }

    public void Move(float moveHorizontal_, float moveVertical_, float speed_)
    {
        if (Debug)
        {
            print("Player: Move");
        }
        
        charController.Move((transform.right * moveHorizontal_ + transform.forward * moveVertical_) * speed_ * Time.deltaTime);
    }

    public void Jump(float height_)
    {
        if (Debug)
        {
            print("Player: Jump");
        }
        
        vel.y = Mathf.Sqrt(height_ * -2.0f * gravity);
    }

    public void Crouch()
    {
        camera.transform.position = new Vector3(crouchPoint.transform.position.x, crouchPoint.transform.position.y, crouchPoint.transform.position.z);
        vel.y += (gravity * 2) * Time.deltaTime;
        bounceHeight = bounceHeight * 2;
    }
}
