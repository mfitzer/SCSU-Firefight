using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Renderer))]
[RequireComponent(typeof (Button))]
public class InterfaceButton : MonoBehaviour
{
    Renderer rend;
    Material material;
    Button button;

    [Tooltip("How long the color of the button should be changed to show it is pressed.")]
    public float pressedFeedbackDuration = 0.25f;

    UIManager uiManager;

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

    public void onHoverStart(Color hoverColor)
    {
        //Debug.Log("<color=purple>Hover start " + gameObject.name + "</color>");
        setMaterialColor(hoverColor);
    }

    public void onHoverStop(Color neutralColor)
    {
        //Debug.Log("<color=purple>Hover stop " + gameObject.name + "</color>");
        setMaterialColor(neutralColor);
    }

    public void press()
    {
        Debug.Log("<color=purple>Press " + gameObject.name + "</color>");
        setMaterialColor(uiManager.buttonPressedColor);
        StartCoroutine(changeColor(uiManager.primaryColor, pressedFeedbackDuration));
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
}
