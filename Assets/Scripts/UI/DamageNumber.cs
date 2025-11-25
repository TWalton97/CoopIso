using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI text;

    [Header("Animation")]
    public float lifetime = 1f;
    public float floatSpeed = 1f;
    public Vector3 randomOffset = new Vector3(0.4f, 0.4f, 0);

    private float timer;
    private Color startColor;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        startColor = text.color;

        // Add a small random offset so numbers aren't stacked
        transform.position += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );
    }

    public void Initialize(int amount, bool isCritical)
    {
        text.text = amount.ToString();
        if (isCritical)
            text.color = Color.red;
    }

    void Update()
    {
        // Always face the camera
        transform.forward = cam.forward;

        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        timer += Time.deltaTime;
        float t = timer / lifetime;
        text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

        // Destroy after lifetime
        if (timer >= lifetime)
            Destroy(gameObject);
    }
}

