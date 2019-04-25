using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    Transform target;

    private void Start()
    {
        if (Camera.main)
        {
            target = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - target.position, Vector3.up);
        }
        else if (Camera.main)
        {
            target = Camera.main.transform;
        }
    }
}
