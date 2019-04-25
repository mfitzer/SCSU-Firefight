using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMatcher : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
        else
        {
            Debug.Log("<color=red>TransformMatcher target is null</color>");
        }
    }
}
