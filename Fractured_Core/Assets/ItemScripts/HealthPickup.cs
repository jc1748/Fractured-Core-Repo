using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    public int healAmount = 3;
    public float floatSpeed = 1f;
    public float floatHeight = 0.2f;
    private float offset;

    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos= transform.position;
        offset = Random.Range(0f,2f*Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        //floating animation
        //math.sin outputs a value between -1 and +1 repeatedly overtime
        transform.position= startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatHeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("player touched the health pickup");
            collision.GetComponent<PlayerHealth>()?.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
