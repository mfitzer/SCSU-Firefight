using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class HololensConfigController : MonoBehaviour
{
    private static HololensConfigController instance;
    public static HololensConfigController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<HololensConfigController>();
            return instance;
        }
    }

    enum ConfigState { SET_POSITION, SET_ROTATION, CLEAR }
    ConfigState configState = ConfigState.SET_POSITION;

    public Transform anchorParent;
    public Transform holoLens;

    public TextMesh logText;

    public bool airTap = false;
    public bool clearAnchor = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void airTapHandler()
    {
        switch (configState)
        {
            case ConfigState.SET_POSITION:
                setPosition();
                break;
            case ConfigState.SET_ROTATION:
                setRotation();
                break;
            case ConfigState.CLEAR:
                clear();
                break;
        }
    }

    //Set ISELF position to cursor position
    void setPosition()
    {
        anchorParent.position = UIManager.Instance.getAirTapHit();
        configState = ConfigState.SET_ROTATION;
    }

    //Update ISELF rotation to match HoloLens around y-axis
    void updateRotation()
    {
        if (configState == ConfigState.SET_ROTATION)
        {
            Vector3 holoLensRotation = holoLens.rotation.eulerAngles;
            Vector3 anchorParentRotation = anchorParent.rotation.eulerAngles;
            anchorParent.rotation = Quaternion.Euler(anchorParentRotation.x, holoLensRotation.y, anchorParentRotation.z);
        }
    }

    //Set ISELF rotation and save world anchor
    void setRotation()
    {
        WorldAnchorManager.Instance.saveAnchor(anchorParent.gameObject);
        configState = ConfigState.CLEAR;
    }

    //Delete ISELF world anchor
    void clear()
    {
        WorldAnchorManager.Instance.deleteAnchor(anchorParent.gameObject);
        configState = ConfigState.SET_POSITION;
    }

    public void logMessage(string msg)
    {
        logText.text = msg;
    }

    // Update is called once per frame
    void Update()
    {
        updateRotation();

        if (airTap)
        {
            airTapHandler();
            airTap = false;
        }

        if (clearAnchor)
        {
            WorldAnchorManager.Instance.deleteAnchor(anchorParent.gameObject);
            clearAnchor = false;
        }
    }
}
