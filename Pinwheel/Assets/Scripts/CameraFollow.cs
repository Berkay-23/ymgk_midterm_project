using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;
    public Vector3 offset;
    public Vector3 targetedPosition;
    public float smoothSpeed = 0.0f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        targetedPosition = targetObject.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetedPosition, ref velocity, smoothSpeed);

    }

}
