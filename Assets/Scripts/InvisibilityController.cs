using System.Collections.Generic;
using UnityEngine;

public class InvisibilityController : MonoBehaviour
{
    public Material invisibleMaterial;

    Dictionary<Transform, Material> originalMaterials;

    bool isInvisible = false;

    public bool toggle = false;

    private void Awake()
    {
        originalMaterials = new Dictionary<Transform, Material>();
        getChildMaterials(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        makeInvisible();
    }

    public void toggleInvisibility()
    {
        isInvisible = !isInvisible;

        setChildMaterials(transform, isInvisible);
    }

    public void makeInvisible()
    {
        setChildMaterials(transform, true);
        isInvisible = true;
    }

    public void makeVisible()
    {
        setChildMaterials(transform, false);
        isInvisible = false;
    }

    void setChildMaterials(Transform parent, bool makeInvisible)
    {
        Renderer rend = parent.GetComponent<Renderer>();
        Material mat = invisibleMaterial;

        if (rend)
        {
            mat = makeInvisible ? invisibleMaterial : originalMaterials[parent];
            rend.material = mat;
        }

        //Set materials in children
        foreach (Transform child in parent.transform)
        {
            if (child != parent)
            {
                setChildMaterials(child, makeInvisible);
            }
        }
    }

    void getChildMaterials(Transform parent)
    {
        Renderer rend = parent.GetComponent<Renderer>();

        if (rend)
        {
            originalMaterials.Add(parent, rend.material);
            //Debug.Log("Stored material for " + parent.gameObject.name + ": " + rend.material.name);
        }

        //Get materials in children
        foreach (Transform child in parent.transform)
        {
            if (child != parent)
            {
                getChildMaterials(child);
            }
        }
    }

    private void Update()
    {
        if (toggle)
        {
            toggleInvisibility();
            toggle = false;
        }
    }
}
