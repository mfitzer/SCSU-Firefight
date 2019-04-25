using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderController : MonoBehaviour
{
    public Pathfinder pathfinder;

    public bool update = false;

    public Transform destination;

    // Start is called before the first frame update
    void Start()
    {
        //findPath();
    }

    public void updateDestination()
    {
        findPath();
    }

    void findPath()
    {
        if (pathfinder.findPath(destination.position))
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
        if (update)
        {
            findPath();

            update = false;
        }
    }
}
