using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CameraFollow cameraFollow;
    private Coroutine shakeRoutine;

    void Awake()
    {
        cameraFollow = GetComponent<CameraFollow>();
    }

    public void Shake(float duration, float strength)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            Vector2 offset = Random.insideUnitCircle * strength;

            if (cameraFollow != null)
            {
                cameraFollow.SetShakeOffset(new Vector3(offset.x, offset.y, 0f));
            }

            yield return null;
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetShakeOffset(Vector3.zero);
        }

        shakeRoutine = null;
    }
}