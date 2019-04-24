using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HololensGestureHandler : MonoBehaviour
{
    GestureRecognizer gestureRecognizer;

    enum AppState { NORMAL, CONFIG }
    AppState appState = AppState.NORMAL;

    private void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.Tapped += tapHandler;
        gestureRecognizer.StartCapturingGestures();
    }

    void tapHandler(TappedEventArgs args)
    {
        //UIManager.Instance.airTapGestureHandler();

        HololensConfigController.Instance.airTapHandler();
    }

    void toggleAppState()
    {
        if (appState == AppState.NORMAL)
        {
            exitNormalOp();
            startConfigOp();
        }
        else
        {
            exitConfigOp();
            startNormalOp();
        }
    }

    void startConfigOp()
    {
        appState = AppState.CONFIG;
    }

    void exitConfigOp()
    {

    }

    void startNormalOp()
    {
        appState = AppState.NORMAL;
    }

    void exitNormalOp()
    {

    }
}
