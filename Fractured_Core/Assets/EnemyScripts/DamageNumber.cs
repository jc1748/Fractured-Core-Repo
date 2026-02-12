using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float lifetime = 1.2f;
    public float moveUpSpeed = 0.5f;

    private TextMeshProUGUI text;
    private float timer;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int amount)
    {
        if (text != null)
            text.text = amount.ToString();
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    void Update()
    {
        // Move upward
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // Timer destroy
        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
