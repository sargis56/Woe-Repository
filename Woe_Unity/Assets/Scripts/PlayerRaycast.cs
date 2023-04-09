using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public PlayerController playerController;

    public float forwardRayDistance = 10.0f;
    public float forwardRayHeight = 0.05f;

    RaycastHit hitForwardData;

    Vector3 forwardM;

    Quaternion forwardMAngle;

    Ray rayForwardM;

    public LayerMask buttonLayerMask;
    public LayerMask deconButtonLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetupRays();
    }

    void SetupRays()
    {
        //Forward ray setup
        forwardMAngle = Quaternion.AngleAxis(0.0f, new Vector3(0, -1, 0));
        forwardM = forwardMAngle * (transform.forward + new Vector3(0, forwardRayHeight, 0));
        rayForwardM = new Ray(transform.position, forwardM);

        Debug.DrawRay(transform.position, forwardM * forwardRayDistance, Color.white);

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, playerController.GetComponent<PlayerController>().monsterLayerMask))
        {
            playerController.GetComponent<PlayerController>().monsterInRange = true;
            playerController.GetComponent<PlayerController>().objectForward = hitForwardData.collider.gameObject;
            if ((playerController.GetComponent<PlayerController>().currentItem == PlayerController.ItemState.Spray) && (playerController.GetComponent<PlayerController>().monster.GetComponent<MonsterController>().currentState != MonsterController.MonsterState.Retreat))
            {
                playerController.GetComponent<PlayerController>().objectForward.GetComponent<MonsterController>().currentState = MonsterController.MonsterState.Caution;
            }

        }
        else
        {
            playerController.GetComponent<PlayerController>().monsterInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, playerController.GetComponent<PlayerController>().botLayerMask))
        {
            playerController.GetComponent<PlayerController>().botInRange = true;
            playerController.GetComponent<PlayerController>().objectForward = hitForwardData.collider.gameObject;
        }
        else
        {
            playerController.GetComponent<PlayerController>().botInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, playerController.GetComponent<PlayerController>().enemyLayerMask))
        {
            playerController.GetComponent<PlayerController>().enemyInRange = true;
            playerController.GetComponent<PlayerController>().objectForward = hitForwardData.collider.gameObject;
        }
        else
        {
            playerController.GetComponent<PlayerController>().enemyInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, buttonLayerMask))
        {
            playerController.GetComponent<PlayerController>().buttonInRange = true;
            playerController.GetComponent<PlayerController>().objectForward = hitForwardData.collider.gameObject;

            hitForwardData.collider.gameObject.GetComponent<MeshRenderer>().material = playerController.GetComponent<PlayerController>().selectMaterial;
            playerController.GetComponent<PlayerController>().infoText.text = "E: Dial up | Q: Dial down";
        }
        else
        {
            playerController.GetComponent<PlayerController>().buttonInRange = false;
        }

        if (Physics.Raycast(rayForwardM, out hitForwardData, forwardRayDistance, deconButtonLayerMask))
        {
            playerController.GetComponent<PlayerController>().deconButtonInRange = true;
            playerController.GetComponent<PlayerController>().objectForward = hitForwardData.collider.gameObject;

            hitForwardData.collider.gameObject.GetComponent<MeshRenderer>().material = playerController.GetComponent<PlayerController>().selectMaterial;
            if (playerController.GetComponent<PlayerController>().hasPestFlask)
            {
                playerController.GetComponent<PlayerController>().infoText.text = "Use E: Start decontamination";
            }
            else
            {
                playerController.GetComponent<PlayerController>().infoText.text = "No pesticide to start decontamination";
            }
        }
        else
        {
            playerController.GetComponent<PlayerController>().deconButtonInRange = false;
        }

        Debug.DrawRay(transform.position, forwardM * hitForwardData.distance, Color.yellow);
    }
}
