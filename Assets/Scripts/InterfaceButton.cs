using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceButton : MonoBehaviour
{
    Renderer rend;
    Material material;
    Button button;

    [Tooltip("How long the color of the button should be changed to show it is pressed.")]
    public float pressedFeedbackDuration = 0.25f;

    UIManager uiManager;

    public bool highlighted = false;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.Instance;
        rend = GetComponent<Renderer>();

        if (rend)
        {
            material = rend.material;
        }

        button = GetComponent<Button>();
    }

    public virtual void onHoverStart(Color hoverColor)
    {
        //Debug.Log("<color=purple>Hover start " + gameObject.name + "</color>");
        setMaterialColor(hoverColor);
        highlighted = true;
    }

    public virtual void onHoverStop(Color neutralColor)
    {
        //Debug.Log("<color=purple>Hover stop " + gameObject.name + "</color>");
        setMaterialColor(neutralColor);
        highlighted = false;
    }

    public virtual void press()
    {
        //Debug.Log("<color=purple>Press " + gameObject.name + "</color>");
        setMaterialColor(uiManager.buttonPressedColor);
        StartCoroutine(changeColor(uiManager.primaryColor, pressedFeedbackDuration));
        highlighted = false;
        if (button)
        {
            button.onClick.Invoke();
        }
    }

    IEnumerator changeColor(Color color, float delay)
    {
        yield return new WaitForSeconds(delay);

        setMaterialColor(color);
    }

    bool setMaterialColor(Color color)
    {
        if (material)
        {
            material.color = color;
            return true;
        }

        Debug.Log("Failed to setMaterialColor on " + gameObject.name);
        return false;
    }

    protected void setChildMaterialColors(Transform parent, Color color)
    {
        Renderer rend = parent.GetComponent<Renderer>();

        if (rend)
        {
            rend.material.color = color;
        }

        //Set materials in children
        foreach (Transform child in parent.transform)
        {
            if (child != parent)
            {
                setChildMaterialColors(child, color);
            }
        }
    }
}
