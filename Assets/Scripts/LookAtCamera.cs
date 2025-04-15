using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null) {
            // Drehe das Objekt zur Kamera und füge eine 180°-Rotation um die Y-Achse hinzu
            Vector3 direction = Camera.main.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
        }
    }
}
