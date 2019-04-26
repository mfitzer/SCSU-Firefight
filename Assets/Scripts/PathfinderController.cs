using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderController : MonoBehaviour
{
    public Pathfinder pathfinder;

    public bool update = false;

    public Transform target;

    Vector3 destination;

    public List<Transform> exitWaypoints;

    bool findPath = true;
    bool targetIsExit = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updatePath()
    {
        if (findPath)
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
    }

    public void destinationReached()
    {
        target.gameObject.SetActive(false);

        if (!targetIsExit && findPath)
        {
            Transform newTarget = pathfinder.findPath(exitWaypoints);

            if (newTarget)
            {
                target = newTarget;
                targetIsExit = true;
            }
            else
            {
                findPath = false;
            }
        }
        else //Stop finding a path once an exit is reached
        {
            findPath = false;
        }
    }

    public void setWaypoint(Transform newTarget)
    {
        target.gameObject.SetActive(false); //Disable old target
        target = newTarget;
        findPath = true;
        targetIsExit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != target.position && findPath)
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
