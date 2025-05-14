using UnityEngine;

public class FloatingCube : MonoBehaviour
{
    public float floatStrength = 0.5f;  // Höhe der Bewegung
    public float floatSpeed = 1f;       // Geschwindigkeit
    private float originalY;
    private float randomOffset;

    void Start()
    {
        originalY = transform.position.y;
        randomOffset = Random.Range(0f, Mathf.PI * 2); // für natürliche Variation
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = originalY + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatStrength;
        transform.position = pos;
    }
}
