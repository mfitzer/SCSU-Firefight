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

    public LineRenderer lineRenderer;
    public Vector3 destination { get; private set; }
    NavMeshPath path;

    [Tooltip("How close to the destination one must be before the destination has been reached.")]
    public float distanceThreshold = 1f;

    [Tooltip("How often the path is updated.")]
    public float refreshRate = 0f;

    public TextMesh distanceLeftDisplay;
    public Transform distanceLeftDisplayParent;

    UIManager uiManager;

    public float distanceLeftDisplayOffset = 0.25f;

    public PathfinderController pathfinderController;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        //lineRenderer = GetComponent<LineRenderer>();

        uiManager = UIManager.Instance;
        distanceLeftDisplay.text = "";
        distanceLeftDisplay.color = uiManager.primaryColor;
    }

    //Sets the destination of the pathfinder, whether its IDLE or ENROUTE (if ENROUTE, update destination)
    public bool findPath(Vector3 possibleDestination, float positionSampleRange = 1f)
    {
        if (NavMesh.SamplePosition(possibleDestination, out NavMeshHit hit, positionSampleRange, NavMesh.AllAreas)) //Destination is valid (on NavMesh)
        {
            destination = hit.position;
            Debug.Log("new destination: " + destination);

            Debug.Log("<color=green>Destination set</color>");

            if (updatePath())
            {
                if (pathfinderState == PathfinderState.IDLE)
                {
                    StartCoroutine(showPath()); //Only show new path if a path is not arleady being displayed
                }

                return true;
            }
        }
        else
        {
            Debug.Log("<color=red>Destination could not be set</color>");
        }

        return false;
    }

    public Transform findPath(List<Transform> possibleDestinations, float positionSampleRange = 1f)
    {
        Vector3 sourcePos = getSourcePosition();

        if (possibleDestinations.Count > 0)
        {
            Transform closestDestination = possibleDestinations[0];
            float shortestDistance = float.PositiveInfinity;

            foreach (Transform dest in possibleDestinations)
            {
                if (NavMesh.SamplePosition(dest.position, out NavMeshHit navHit, positionSampleRange, NavMesh.AllAreas)) //Destination is valid (on NavMesh)
                {
                    NavMeshPath possiblePath = calculatePath(sourcePos, dest.position);

                    float pathLength = distanceLeft(dest.position, possiblePath);
                    if (possiblePath.status == NavMeshPathStatus.PathComplete && pathLength < shortestDistance)
                    {
                        closestDestination = dest;
                        shortestDistance = pathLength;
                    }

                    Debug.Log("Possible path length: " + pathLength);
                }
            }

            Debug.Log("Shortest distance: " + shortestDistance);


            destination = closestDestination.position;
            closestDestination.gameObject.SetActive(true);
            Debug.Log("new destination: " + destination);
            Debug.Log("<color=green>Destination set</color>");

            if (updatePath())
            {
                if (pathfinderState == PathfinderState.IDLE)
                {
                    StartCoroutine(showPath()); //Only show new path if a path is not arleady being displayed
                }

                return closestDestination;
            }
        }

        return null;
    }

    IEnumerator showPath()
    {
        pathfinderState = PathfinderState.ENROUTE;

        while (distanceLeft(getSourcePosition(), path) > distanceThreshold)
        {
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(refreshRate);

            updatePath();
            //Debug.Log("Update path");
        }

        Debug.Log("<color=purple>Destination reached</color>");

        lineRenderer.enabled = false;
        distanceLeftDisplay.text = "";
        path.ClearCorners();
        pathfinderState = PathfinderState.IDLE;

        pathfinderController.destinationReached();
    }

    bool updatePath()
    {
        NavMeshPath possiblePath = calculatePath(getSourcePosition(), destination);

        if (possiblePath.status == NavMeshPathStatus.PathComplete) //Path found
        {
            path = possiblePath;
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);

            return true;
        }
        else if (possiblePath.status == NavMeshPathStatus.PathInvalid) //Draws path from current position (even off of the NavMesh)
        {
            lineRenderer.positionCount = path.corners.Length + 1;
            lineRenderer.SetPosition(0, transform.position);
            for (int i = 1; i <= path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, path.corners[i - 1]);
            }
        }
        else //PathPartial
        {
            Debug.Log("<color=red>Path cannot reach destination.</color>");
        }

        return false;
    }

    NavMeshPath calculatePath(Vector3 sourcePos, Vector3 targetPos)
    {
        NavMeshPath possiblePath = new NavMeshPath();

        NavMesh.CalculatePath(getSourcePosition(), destination, NavMesh.AllAreas, possiblePath);

        return possiblePath;
    }

    //Position of pathfinder on navmesh
    Vector3 getSourcePosition()
    {
        Vector3 sourcePos = transform.position;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            if (NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas)) //Position is valid (on NavMesh)
            {
                //Debug.Log("Raycast navhit");
                sourcePos = navHit.position;
            }
            else if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //Position is valid (on NavMesh)
            {
                //Debug.Log("Position navhit");
                sourcePos = hit.position;
            }
            else
            {
                //Debug.Log("Raycast");
                sourcePos = hitInfo.point;
            }
        }
        else
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas)) //Position is valid (on NavMesh)
            {
                //Debug.Log("navhit");
                sourcePos = hit.position;
            }
            else
            {
                //Debug.Log("position");
            }
        }

        return sourcePos;
    }

    float distanceLeft(Vector3 targetDestination, NavMeshPath navPath)
    {
        float distance = 0f;

        for (int i = 0; i < navPath.corners.Length - 1; i++)
        {
            if (i == 0)
            {
                distance += Vector3.Distance(targetDestination, navPath.corners[0]);
            }

            distance += Vector3.Distance(navPath.corners[i], navPath.corners[i + 1]);
        }

        return distance;
    }

    void displayDistanceLeft(Vector3 distance)
    {
        distanceLeftDisplay.text = distanceLeft(getSourcePosition(), path).ToString("0.00") + " m"; //Display distance remaining in meters

        Vector3 pathVector = path.corners[1] - path.corners[0];
        //Vector3 displayPos = path.corners[0] + distanceLeftDisplayOffset * pathVector;
        distanceLeftDisplayParent.transform.position = path.corners[0];
        distanceLeftDisplayParent.rotation = Quaternion.LookRotation(pathVector, Vector3.up);
        //Debug.Log("Distance left: " + distance);
    }

    //Updates the color in case it has changed
    void updateUIColors()
    {
        distanceLeftDisplay.color = uiManager.primaryColor;
    }

    private void OnDisable()
    {
        pathfinderState = PathfinderState.IDLE;
    }

    private void Update()
    {
        updateUIColors();
    }
}
