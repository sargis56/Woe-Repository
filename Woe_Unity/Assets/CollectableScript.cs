using Unity.Netcode;
using UnityEngine;

public class CollectableScript : NetworkBehaviour
{
    public bool triggered = false;
    private void Start()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && triggered == false)
        {
            triggered = true;
            transform.position = new Vector3(0.0f, -100.0f, 0.0f);
            gameObject.SetActive(false);
        }
    }
}
