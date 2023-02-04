using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    bool grounded = false;
    Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
    public float velRest = -2.0f;
    public float speed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;

    public CharacterController charController;
    public Transform groundCheck;
    public float distanceFromGround = 0.4f;
    public LayerMask groundLayerMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, distanceFromGround, groundLayerMask);

        if (grounded && vel.y < 0.0f)
        {
            //print("Player: Grounded");
            vel.y = velRest;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetButton("Sprint"))
        {
            print("Player: Sprint");
            Move(moveHorizontal, moveVertical, sprintSpeed);
        }
        else
        {
            Move(moveHorizontal, moveVertical, speed);
        }

        
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump();
        }

        vel.y += gravity * Time.deltaTime;
        charController.Move(vel * Time.deltaTime);
    }

    public void Move(float moveHorizontal_, float moveVertical_, float speed_)
    {
        //print("Player: Move");
        charController.Move((transform.right * moveHorizontal_ + transform.forward * moveVertical_) * speed_ * Time.deltaTime);
    }

    public void Jump()
    {
        print("Player: Jump");
        vel.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
    }
}
