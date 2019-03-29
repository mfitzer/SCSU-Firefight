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

    public bool findPath(Vector3 possibleDestination)
    {
        //if (NavMesh.SamplePosition(possibleDestination, out NavMeshHit hit, distanceThreshold, NavMesh.GetAreaFromName(walkableNavMeshArea))) //Destination is valid (on NavMesh)
        if (NavMesh.SamplePosition(possibleDestination, out NavMeshHit hit, distanceThreshold, NavMesh.AllAreas)) //Destination is valid (on NavMesh)
        {
            destination = hit.position;

            Debug.Log("<color=green>Destination set</color>");

            if (updatePath())
            {
                StartCoroutine(showPath());
                return true;
            }
        }
        else
        {
            Debug.Log("<color=red>Destination could not be set</color>");
        }

        return false;
    }

    IEnumerator showPath()
    {
        pathfinderState = PathfinderState.ENROUTE;

        while (updatePath() && distanceLeft() > distanceThreshold)
        {
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(refreshRate);
        }

        Debug.Log("<color=purple>Destination reached</color>");

        lineRenderer.enabled = false;
        pathfinderState = PathfinderState.IDLE;
    }

    bool updatePath()
    {
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathComplete) //Path found
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);

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
