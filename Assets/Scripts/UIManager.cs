using Microsoft.MixedReality.Toolkit.Services.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }

    enum UIState { IDLE, DISPLAYING_MENU, MARKING_HAZARD, SETTING_WAYPOINT }
    UIState uiState = UIState.IDLE;

    //Colors
    public List<Material> primaryColorMaterials;
    public Color primaryColor;
    public Color buttonHighlightColor;
    public Color buttonPressedColor;

    //UI Interactions
    GazeProvider gazeProvider;
    Vector3 gazeHitpoint;
    InterfaceButton buttonInGaze; //The button being hit by the current gaze raycast
    public LayerMask gazeLayerMask;

    [Tooltip("Max distance UI can be interacted with")]
    public float gazeMaxDistance = 50f;

    public GameObject hazardMarkerPrefab;
    public GameObject waypointMarkerPrefab;
    public Transform menu;

    GameObject markerBeingPlaced;

    public bool updateColors = true;

    //TESTING
    public bool showMenu = false;
    public bool hideMenu = false;
    public bool markHazard = false;
    public bool setWaypoint = false;
    public bool pressBtn = false;

    private void Start()
    {
        gazeProvider = FindObjectOfType<GazeProvider>();
        closeMenu();
    }

    void processGaze()
    {
        RaycastHit hitInfo;
        if (gazeProvider)
        {
            if (Physics.Raycast(gazeProvider.GazeOrigin, gazeProvider.GazeDirection, out hitInfo, gazeMaxDistance, gazeLayerMask)) //Gaze hit something
            {
                //Debug.Log("<color=blue>Unity Gaze hit: " + hitInfo.collider.name + "</color>");
                gazeHitpoint = hitInfo.point; //Store hitInfo until next frame

                if (uiState == UIState.DISPLAYING_MENU)
                {
                    GameObject objHit = hitInfo.collider.gameObject;
                    if (buttonInGaze && buttonInGaze.gameObject != objHit) //Hit a button last frame and objHit is not that button
                    {
                        buttonInGaze.onHoverStop(primaryColor); //Was hovering on button
                    }

                    buttonInGaze = objHit.GetComponent<InterfaceButton>();

                    if (buttonInGaze) //objHit is an InterfaceButton
                    {
                        buttonInGaze.onHoverStart(buttonHighlightColor);
                    }
                }
                else if (uiState == UIState.MARKING_HAZARD || uiState == UIState.SETTING_WAYPOINT)
                {
                    if (markerBeingPlaced && gazeProvider.HitInfo.collider.gameObject != markerBeingPlaced) //Marker is not null && not looking at the marker
                    {
                        markerBeingPlaced.transform.position = gazeHitpoint;
                    }
                }
            }
            else
            {
                //Debug.Log("<color=red>Gaze hit nothing</color>");
            }
        }
        else
        {
            //Debug.Log("<color=red>Gaze Provider is null</color>");
        }
    }

    #region UI Menu Events

    public void startHazardMarkerPlacement()
    {
        Debug.Log("<color=red>Start hazard marker placement</color>");
        closeMenu();
        uiState = UIState.MARKING_HAZARD;

        //Display airtap GIF

        markerBeingPlaced = Instantiate(hazardMarkerPrefab, gazeHitpoint, hazardMarkerPrefab.transform.rotation);
        Collider collider = markerBeingPlaced.GetComponent<Collider>();
        if (collider) //Disable collider
        {
            collider.enabled = false;
        }
    }

    public void placeHazardMarker()
    {
        if (uiState == UIState.MARKING_HAZARD)
        {
            uiState = UIState.IDLE;
            Collider collider = markerBeingPlaced.GetComponent<Collider>();
            if (collider) //Enable collider
            {
                collider.enabled = true;
            }
            markerBeingPlaced = null;
        }
    }

    public void startWaypointMarkerPlacement()
    {
        Debug.Log("<color=blue>Start waypoint placement</color>");
        closeMenu();
        uiState = UIState.SETTING_WAYPOINT;

        //Display airtap GIF

        markerBeingPlaced = Instantiate(waypointMarkerPrefab, gazeHitpoint, waypointMarkerPrefab.transform.rotation);
        Collider collider = markerBeingPlaced.GetComponent<Collider>();
        if (collider) //Disable collider
        {
            collider.enabled = false;
        }
    }

    public void placeWaypointMarker()
    {
        if (uiState == UIState.SETTING_WAYPOINT)
        {
            uiState = UIState.IDLE;
            Collider collider = markerBeingPlaced.GetComponent<Collider>();
            if (collider) //Enable collider
            {
                collider.enabled = true;
            }
            markerBeingPlaced = null;
        }
    }

    public void openMenu()
    {
        Vector3 targetRotation = gazeProvider.transform.eulerAngles;
        Vector3 currentRotation = menu.eulerAngles;
        menu.eulerAngles = new Vector3(currentRotation.x, targetRotation.y, currentRotation.z); //Match rotation around the y-axis
        menu.gameObject.SetActive(true);
        uiState = UIState.DISPLAYING_MENU;
    }

    public void closeMenu()
    {
        menu.gameObject.SetActive(false);
        uiState = UIState.IDLE;
    }

    void pressButton()
    {
        if (buttonInGaze) //Button is in gaze
        {
            buttonInGaze.press();
        }
        else
        {
            Debug.Log("<color=red>There is no button in gaze to press</color>");
        }
    }

    #endregion UI Menu Events

    #region Hololens Input Events

    //Handles the event of an air tap gesture being performed
    public void airTapGestureHandler()
    {
        switch (uiState)
        {
            case UIState.IDLE:
                openMenu();
                break;
            case UIState.DISPLAYING_MENU:
                pressButton();
                break;
            case UIState.MARKING_HAZARD:
                placeHazardMarker();
                break;
            case UIState.SETTING_WAYPOINT:
                placeWaypointMarker();
                break;
        }
    }

    #endregion Hololens Input Events

    void updateMaterialColors(List<Material> materials, Color color)
    {
        foreach (Material mat in materials)
        {
            mat.color = color;
        }
    }

    void Update()
    {
        processGaze();

        if (updateColors)
        {
            updateMaterialColors(primaryColorMaterials, primaryColor);
            updateColors = false;
        }

        //TESTING
        if (markHazard)
        {
            placeHazardMarker();
            markHazard = false;
        }

        if (setWaypoint)
        {
            placeWaypointMarker();
            setWaypoint = false;
        }

        if (pressBtn)
        {
            pressButton();
            pressBtn = false;
        }

        if (showMenu)
        {
            openMenu();
            showMenu = false;
        }

        if (hideMenu)
        {
            closeMenu();
            hideMenu = false;
        }        
    }
}
