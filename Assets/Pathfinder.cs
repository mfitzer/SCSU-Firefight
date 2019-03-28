using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (LineRenderer))]
public class Pathfinder : MonoBehaviour
{
    enum PathfinderState { IDLE, ENROUTE }
    PathfinderState pathfinderState = PathfinderState.IDLE;

    const string walkableNavMeshArea = "Walkable";

    LineRenderer lineRenderer;
    Vector3 destination;
    NavMeshPath path;

    [Tooltip("How close to the destination one must be before the destination has been reached.")]
    public float distanceThreshold = 1f;

    [Tooltip("How often the path is updated.")]
    public float refreshRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public bool setDestination(Vector3 possibleDestination)
    {
        if (NavMesh.SamplePosition(possibleDestination, out NavMeshHit hit, 1f, NavMesh.GetAreaFromName(walkableNavMeshArea))) //Destination is valid (on NavMesh)
        {
            destination = hit.position;
            StartCoroutine(showPath());
            return true;
        }

        return false;
    }

    IEnumerator showPath()
    {
        pathfinderState = PathfinderState.ENROUTE;
        bool pathValid = true;

        while (pathValid && distanceLeft() > distanceThreshold)
        {
            pathValid = updatePath();
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(refreshRate);
        }

        lineRenderer.enabled = false;
        pathfinderState = PathfinderState.IDLE;
    }

    bool updatePath()
    {
        NavMesh.CalculatePath(transform.position, destination, NavMesh.GetAreaFromName(walkableNavMeshArea), path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            //Update line renderer points
            return true;
        }

        return false;
    }

    float distanceLeft() //This could probably be optimized
    {
        float distance = 0f;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return distance;
    }
}
