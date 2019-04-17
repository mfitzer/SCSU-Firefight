using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFrameScaler : MonoBehaviour
{
    public TextMesh textMesh;
    public float xScaleFactor = 1;
    public float yScaleFactor = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void updateScale()
    {
        Vector3 textBounds = textMesh.GetComponent<Renderer>().bounds.size;
        transform.localScale = new Vector3(textBounds.x * xScaleFactor, textBounds.y * yScaleFactor, 1);
    }

    // Update is called once per frame
    void Update()
    {
        updateScale();
    }
}
