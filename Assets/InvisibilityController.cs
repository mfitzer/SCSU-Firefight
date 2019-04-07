using UnityEngine;

public class InvisibilityController : MonoBehaviour
{
    public Material invisibleMaterial;

    // Start is called before the first frame update
    void Start()
    {
        enableInvisibility(transform);
    }

    public void enableInvisibility(Transform parent)
    {
        Renderer rend = parent.GetComponent<Renderer>();

        if (rend)
        {
            rend.material = invisibleMaterial;
        }

        //Change materials in children
        foreach (Transform child in parent.transform)
        {
            if (child != parent)
            {
                enableInvisibility(child);
            }
        }
    }
}
