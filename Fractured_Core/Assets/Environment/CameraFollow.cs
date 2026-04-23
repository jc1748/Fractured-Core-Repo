using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [HideInInspector] public bool followEnabled = true;

    private Vector3 shakeOffset = Vector3.zero;

    public void SetShakeOffset(Vector3 offsetAmount)
    {
        shakeOffset = offsetAmount;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        FindTargetIfMissing();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindTargetIfMissing();
            return;
        }

        if (!followEnabled)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            transform.position.y,
            offset.z
        );

        Vector3 followPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = followPosition + shakeOffset;
    }

    void FindTargetIfMissing()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
}