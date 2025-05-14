using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea))]
public class FloatingPlatformTeleport : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatStrength = 0.5f;  // Höhe der Bewegung
    public float floatSpeed = 1f;       // Geschwindigkeit
    
    [Header("Teleport Settings")]
    [Tooltip("Das XR Rig oder Hauptobjekt des Spielers")]
    public Transform playerTransform;
    
    // Private Variablen
    private float originalY;
    private float randomOffset;
    private Transform playerOriginalParent;
    private bool isPlayerAttached = false;
    private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea teleportArea;

    void Start()
    {
        // Plattform-Bewegungs-Initialisierung
        originalY = transform.position.y;
        randomOffset = Random.Range(0f, Mathf.PI * 2); // für natürliche Variation
        
        // Teleport-Komponenten Setup
        SetupTeleportArea();
        
        // Automatische Player-Erkennung
        if (playerTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null && mainCamera.transform.parent != null)
            {
                playerTransform = mainCamera.transform.parent.parent; // Typischerweise XR Rig
                Debug.Log("Player Transform automatisch gefunden: " + playerTransform.name);
            }
        }
    }

    void Update()
    {
        // Plattform-Bewegung
        Vector3 pos = transform.position;
        float newY = originalY + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatStrength;
        
        // Bewege die Plattform
        pos.y = newY;
        transform.position = pos;
    }
    
    private void SetupTeleportArea()
    {
        // Stelle sicher, dass wir eine TeleportationArea haben
        teleportArea = GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();
        if (teleportArea == null)
        {
            teleportArea = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();
        }
        
        // Überprüfe und füge bei Bedarf einen Collider hinzu
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Füge einen BoxCollider hinzu und passe ihn an die Geometrie an
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.size = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z);
            boxCol.center = new Vector3(0, 0, 0);
            boxCol.isTrigger = true; // Als Trigger markieren für die Teleportation
        }
        else
        {
            // Stelle sicher, dass der Collider als Trigger markiert ist
            col.isTrigger = true;
        }
        
        // Optional: Stelle sicher, dass die Plattform die richtige Layer hat
        gameObject.layer = LayerMask.NameToLayer("Teleport");
    }
    
    // Player-Attachment-Logik für die Bewegung mit der Plattform
    private void OnTriggerEnter(Collider other)
    {
        AttachPlayerIfNeeded(other.gameObject);
    }
    
    private void AttachPlayerIfNeeded(GameObject obj)
    {
        if (playerTransform != null && !isPlayerAttached)
        {
            // Prüfe, ob es der Spieler oder ein Teil davon ist
            bool isPlayerObject = (obj.transform == playerTransform);
            
            // Oder prüfe auf relevante Kind-Objekte
            if (!isPlayerObject && playerTransform != null)
            {
                if (obj.GetComponent<Camera>() != null && obj.transform.IsChildOf(playerTransform))
                {
                    isPlayerObject = true;
                }
                
                if (!isPlayerObject && obj.GetComponent<CharacterController>() != null && 
                    (obj.transform == playerTransform || obj.transform.IsChildOf(playerTransform)))
                {
                    isPlayerObject = true;
                }
            }
            
            if (isPlayerObject)
            {
                // Spieler anheften
                playerOriginalParent = playerTransform.parent;
                Vector3 originalWorldPos = playerTransform.position;
                playerTransform.SetParent(transform);
                playerTransform.position = originalWorldPos; // Weltposition beibehalten
                isPlayerAttached = true;
                Debug.Log("Spieler an Plattform angeheftet");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        DetachPlayerIfNeeded(other.gameObject);
    }
    
    private void DetachPlayerIfNeeded(GameObject obj)
    {
        if (playerTransform != null && isPlayerAttached)
        {
            // Ähnliche Prüfung wie beim Betreten
            bool isPlayerObject = (obj.transform == playerTransform);
            
            if (!isPlayerObject && playerTransform != null)
            {
                if (obj.GetComponent<Camera>() != null && obj.transform.IsChildOf(playerTransform))
                {
                    isPlayerObject = true;
                }
                
                if (!isPlayerObject && obj.GetComponent<CharacterController>() != null && 
                    (obj.transform == playerTransform || obj.transform.IsChildOf(playerTransform)))
                {
                    isPlayerObject = true;
                }
            }
            
            if (isPlayerObject)
            {
                // Spieler lösen
                Vector3 originalWorldPos = playerTransform.position;
                playerTransform.SetParent(playerOriginalParent);
                playerTransform.position = originalWorldPos; // Weltposition beibehalten
                isPlayerAttached = false;
                Debug.Log("Spieler von Plattform gelöst");
            }
        }
    }
}