using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderController : MonoBehaviour
{
    public Pathfinder pathfinder;

    public bool update = false;

    public Transform target;
    Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updatePath()
    {
        destination = target.position;
        if (pathfinder.findPath(destination))
        {
            Debug.Log("<color=green>Path found</color>");
        }
        else
        {
            Debug.Log("<color=red>Path not found</color>");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != target.position)
        {
            updatePath();
        }

        //Testing
        if (update)
        {
            updatePath();

            update = false;
        }
    }
}
