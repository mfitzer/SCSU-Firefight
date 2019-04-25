using UnityEngine;
using UnityEngine.XR.WSA.Input;
using System.Collections;
using System.Collections.Generic;

public class HololensGestureHandler : MonoBehaviour
{
    GestureRecognizer gestureRecognizer;

    enum AppState { NORMAL, CONFIG }
    AppState appState = AppState.NORMAL;

    public List<GameObject> configObjs;
    public List<GameObject> normalObjs;

    public InvisibilityController ISELFInvisibility;
    public PathfinderController pathfinderController;

    public bool airTap = false;
    public bool hold = false;

    private void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);
        gestureRecognizer.Tapped += tapHandler;
        gestureRecognizer.HoldCompleted += holdHandler;
        gestureRecognizer.StartCapturingGestures();

        startNormalOp();
    }

    void tapHandler(TappedEventArgs args)
    {
        tap();
    }

    void tap()
    {
        switch (appState)
        {
            case AppState.NORMAL:
                UIManager.Instance.airTapGestureHandler();
                break;
            case AppState.CONFIG:
                HololensConfigController.Instance.airTapHandler();
                break;
        }
    }

    void holdHandler(HoldCompletedEventArgs args)
    {
        toggleAppState();
    }

    void toggleAppState()
    {
        if (appState == AppState.NORMAL)
        {
            startConfigOp();
        }
        else
        {
            startNormalOp();
        }

        HololensConfigController.Instance.logMessage(appState + " mode enabled");
    }

    void startConfigOp()
    {
        exitNormalOp();

        appState = AppState.CONFIG;
        enableGameObjects(configObjs);
        ISELFInvisibility.makeVisible();
    }

    void exitConfigOp()
    {
        disableGameObjects(configObjs);
    }

    void startNormalOp()
    {
        exitConfigOp();

        appState = AppState.NORMAL;
        enableGameObjects(normalObjs);
        ISELFInvisibility.makeInvisible();
        pathfinderController.updateDestination();
    }

    void exitNormalOp()
    {
        disableGameObjects(normalObjs);
    }

    #region Helpers

    void disableGameObjects(List<GameObject> objs)
    {
        foreach (GameObject obj in objs)
        {
            obj.SetActive(false);
        }
    }

    void enableGameObjects(List<GameObject> objs)
    {
        foreach (GameObject obj in objs)
        {
            obj.SetActive(true);
        }
    }

    #endregion Helpers

    private void Update()
    {
        if (airTap)
        {
            tap();
            airTap = false;
        }
        else if (hold)
        {
            toggleAppState();
            hold = false;
        }
    }
}
