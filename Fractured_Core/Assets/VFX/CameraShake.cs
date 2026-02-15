using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    //call this from anywhere to shake the camera
    public void Shake(float duration, float strength)
    {
        //if already shaking, restart it(feels consistent)
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

            //random offset inside small circle
            Vector2 offset = Random.insideUnitCircle * strength;

            transform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        //return camera to original position
        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
