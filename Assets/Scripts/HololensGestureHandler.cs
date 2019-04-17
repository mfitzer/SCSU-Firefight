using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HololensGestureHandler : MonoBehaviour
{
    GestureRecognizer gestureRecognizer;

    private void Start()
    {
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        gestureRecognizer.Tapped += tapHandler;
        gestureRecognizer.StartCapturingGestures();
    }

    void tapHandler(TappedEventArgs args)
    {
        UIManager.Instance.airTapGestureHandler();
    }
}
