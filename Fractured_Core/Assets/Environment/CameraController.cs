using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    public bool followX = true;
    public bool followY = false;

    [Header("Camera Bounds")]
    public bool useBounds = true;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private bool isLocked = false;
    private Vector3 lockedPosition;

    void Start()
    {
        isLocked = false;
        FindPlayerIfMissing();
    }

    void LateUpdate()
    {
        Debug.Log("Camera LateUpdate running. isLocked = " + isLocked + " player = " + player);

        if (isLocked)
        {
            transform.position = lockedPosition;
            return;
        }

        FindPlayerIfMissing();

        if (player == null)
        {
            Debug.LogWarning("CameraController: Player is missing.");
            return;
        }

        Vector3 targetPosition = transform.position;

        if (followX)
            targetPosition.x = player.position.x;

        if (followY)
            targetPosition.y = player.position.y;

        targetPosition.z = transform.position.z;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(
        transform.position,
        targetPosition,
        followSpeed * Time.deltaTime
        );
    }

    public void LockCamera(Vector3 position)
    {
        isLocked = true;

        lockedPosition = new Vector3(
            position.x,
            position.y,
            transform.position.z
        );

        Debug.Log("Camera locked.");
    }

    public void UnlockCamera()
    {
        isLocked = false;
        Debug.Log("Camera unlocked on: " + gameObject.name);
    }

    private void FindPlayerIfMissing()
    {
        if (player != null) return;

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            Debug.Log("CameraController found player.");
        }
    }
}