using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardMarker : InterfaceButton
{
    public InterfaceButton removeButton;
    public Transform icon;
    public GameObject markerParent;

    bool hidingButton = false;

    public float hideButtonDelay = 5f;

    private void Start()
    {
        removeButton.gameObject.SetActive(false);
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
        removeButton.gameObject.SetActive(!removeButton.gameObject.activeSelf);
        
        if (removeButton.gameObject.activeSelf)
        {
            StartCoroutine(hideRemoveButton(hideButtonDelay));
        }
    }

    public void removeMarker()
    {
        Destroy(markerParent);
    }

    IEnumerator hideRemoveButton(float delay)
    {
        hidingButton = true;

        float timeWaited = 0f;

        while (timeWaited < delay)
        {
            yield return null;

            if (removeButton.highlighted)
            {
                timeWaited = 0;
            }
            else
            {
                timeWaited += Time.deltaTime;
            }
        }

        removeButton.gameObject.SetActive(false);
        hidingButton = false;
    }
}
