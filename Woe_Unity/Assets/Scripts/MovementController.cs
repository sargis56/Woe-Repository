using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovementController : MonoBehaviour
{
    public TextMeshProUGUI staminaText;

    public bool debug = false;

    bool grounded = false;
    bool bounce = false;
    public bool safe = false;
    bool tired = false;
    public float sprintTimer = 0.0f;
    public float sprintSeconds = 5.0f;

    Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
    public float velRest = -2.0f;
    public float speed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;
    public float bounceHeight = 1.0f;
    public float bounceHeight_ORG;

    public CharacterController charController;
    float charControllerX_ORG;

    //public GameObject camera;
    public Transform groundCheck;
    public Transform spawnPoint;
    public Transform crouchPoint;
    public Transform straightPoint;
    public float distanceFromGround = 0.4f;
    public LayerMask groundLayerMask;
    public LayerMask bounceLayerMask;
    public LayerMask roomLayerMask;
    public LayerMask safeZoneLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        bounceHeight_ORG = bounceHeight;
        charControllerX_ORG = charController.center.x;
    }

    // Update is called once per frame
    void Update()
    {
        // staminaText.text = "Stamina: " + sprintTimer.ToString();

        grounded = Physics.CheckSphere(groundCheck.position, distanceFromGround, groundLayerMask);
        bounce = Physics.CheckSphere(groundCheck.position, distanceFromGround, bounceLayerMask);
        safe = Physics.CheckSphere(groundCheck.position, distanceFromGround, safeZoneLayerMask);

        if (grounded && vel.y < 0.0f)
        {
            if (debug)
            {
                print("Player: Grounded");
            }
            vel.y = velRest;
        }

        if (bounce && vel.y < 0.0f)
        {
            if (debug)
            {
                print("Player: Bounce");
            }
            Jump(bounceHeight);
        }
        bounceHeight = bounceHeight_ORG;

        charController.center = new Vector3(0, charControllerX_ORG, 0);
        //camera.transform.position = new Vector3(straightPoint.position.x, straightPoint.position.y, straightPoint.position.z);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetButton("Crouch"))
        {
            if (debug)
            {
                print("Player: Crouch");
            }
            
            Crouch();

        }

        if (Input.GetButton("Sprint") && !tired)
        {
            if (debug)
            {
                print("Player: Sprint");
            }

            sprintTimer += Time.deltaTime;

            if (sprintTimer < sprintSeconds)
            {
                Move(moveHorizontal, moveVertical, sprintSpeed);
            }
            else
            {
                tired = true;
            }
            
        }
        else
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0)
            {
                sprintTimer = 0;
                tired = false;
            }
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
        if (debug)
        {
            print("Player: Move");
        }
        
        charController.Move((transform.right * moveHorizontal_ + transform.forward * moveVertical_) * speed_ * Time.deltaTime);
    }

    public void Jump(float height_)
    {
        if (debug)
        {
            print("Player: Jump");
        }
        
        vel.y = Mathf.Sqrt(height_ * -2.0f * gravity);
    }

    public void Crouch()
    {
        charController.center = new Vector3(0, 0.5f, 0);
        //camera.transform.position = new Vector3(crouchPoint.transform.position.x, crouchPoint.transform.position.y, crouchPoint.transform.position.z);
        vel.y += (gravity * 2) * Time.deltaTime;
        bounceHeight = bounceHeight * 2;
    }
}
