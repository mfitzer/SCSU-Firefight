using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardMarker : InterfaceButton
{
    public GameObject removeButton;
    public Transform icon;

    private void Start()
    {
        removeButton.SetActive(false);
    }

    public override void onHoverStart(Color hoverColor)
    {
        //Debug.Log("On hover start hazard marker");
        setChildMaterialColors(icon, hoverColor);
    }

    public override void onHoverStop(Color neutralColor)
    {
        //Debug.Log("On hover stop hazard marker");
        setChildMaterialColors(icon, neutralColor);
    }

    public override void press()
    {
        removeButton.SetActive(!removeButton.activeSelf);
    }

    public void removeMarker()
    {
        Destroy(gameObject);
    }
}
