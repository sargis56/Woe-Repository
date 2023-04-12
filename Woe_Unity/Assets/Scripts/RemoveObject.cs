using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObject : MonoBehaviour
{
    [SerializeField]
    private float lingerTime = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lingerTime -= Time.deltaTime;
        if (lingerTime < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
