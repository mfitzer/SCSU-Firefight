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

            Debug.Log("<color=green>Destination set</color>");

            if (pathfinderState == PathfinderState.IDLE && updatePath())
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

        while (distanceLeft() > distanceThreshold)
        {
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(refreshRate);

            updatePath();
        }

        Debug.Log("<color=purple>Destination reached</color>");

        lineRenderer.enabled = false;
        distanceLeftDisplay.text = "";
        path.ClearCorners();
        pathfinderState = PathfinderState.IDLE;
    }

    bool updatePath()
    {
        NavMeshPath possiblePath = new NavMeshPath();
        NavMesh.CalculatePath(getSourcePosition(), destination, NavMesh.AllAreas, possiblePath);

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

    //Position of pathfinder on navmesh
    Vector3 getSourcePosition()
    {
        Vector3 sourcePos = transform.position;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            sourcePos = hitInfo.point;
        }

        return sourcePos;
    }

    float distanceLeft()
    {
        float distance = 0f;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            if (i == 0)
            {
                distance += Vector3.Distance(transform.position, path.corners[0]);
            }

            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        distanceLeftDisplay.text = distance.ToString("0.00") + " m"; //Display distance remaining in meters

        Vector3 pathVector = path.corners[1] - path.corners[0];
        Vector3 displayPos = path.corners[0] + distanceLeftDisplayOffset * pathVector;
        distanceLeftDisplayParent.transform.position = displayPos;
        //Debug.Log("Distance left: " + distance);

        return distance;
    }

    //Updates the color in case it has changed
    void updateUIColors()
    {
        distanceLeftDisplay.color = uiManager.primaryColor;
    }

    private void Update()
    {
        updateUIColors();
    }
}
