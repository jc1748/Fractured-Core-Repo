using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine shakeRoutine;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            transform.localPosition = originalLocalPosition;
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

            // Actually move the camera locally
            transform.localPosition = originalLocalPosition + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        // Reset camera after shake
        transform.localPosition = originalLocalPosition;

        shakeRoutine = null;
    }
}