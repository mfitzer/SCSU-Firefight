using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    [Tooltip("Transform to follow.")]
    public Transform target;

    [Header("Translation")]
    public bool matchPosition = true;
    [Tooltip("Speed at which the target will be followed if matchPosition is checked.")]
    [Range(0f, 1f)]
    public float translationSpeed = 1f;

    [Header("Rotation")]
    public bool matchRotation = true;
    [Tooltip("Speed at which the rotation of the target will be matched if matchRotation is checked.")]
    [Range(0f, 1f)]
    public float rotationSpeed = 0.5f;

    //Follow target position in xz-plane
    void followPosition()
    {
        Vector3 position = transform.position;
        Vector3 targetPosition = target.position;

        if (matchPosition)
        {
            Vector3 lerpPosition = Vector3.Lerp(position, targetPosition, translationSpeed);
            Vector3 newPosition = new Vector3(lerpPosition.x, lerpPosition.y, lerpPosition.z);
            transform.position = newPosition;
        }
    }

    //Follow target rotation around y-axis
    void followRotation()
    {
        if (matchRotation)
        {
            Vector3 targetRotationEuler = target.rotation.eulerAngles;
            Vector3 currentRotationEuler = transform.rotation.eulerAngles;
            Quaternion yAxisRotation = Quaternion.Euler(currentRotationEuler.x, targetRotationEuler.y, currentRotationEuler.z);
            Quaternion lerpRotation = Quaternion.Lerp(transform.rotation, yAxisRotation, rotationSpeed);
            transform.rotation = lerpRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        followPosition();

        followRotation();
    }
}
