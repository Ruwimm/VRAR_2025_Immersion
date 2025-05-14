using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportPlayerAttach : MonoBehaviour
{
    [Tooltip("Das XR Rig oder das Hauptobjekt des Spielers")]
    public Transform playerTransform;
    
    private Transform playerOriginalParent;
    private bool isPlayerAttached = false;
    private GameObject attachPoint;

    private void Start()
    {
        // Wenn kein playerTransform zugewiesen wurde, versuche die Kamera zu finden
        if (playerTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Versuche das XR Rig zu finden (üblicherweise das Elternobjekt der Kamera oder dessen Eltern)
                Transform parent = mainCamera.transform.parent;
                if (parent != null)
                {
                    // Gehe eine Ebene höher, falls "Camera Offset" das direkte Elternteil ist
                    if (parent.name.Contains("Offset") && parent.parent != null)
                    {
                        playerTransform = parent.parent;
                    }
                    else
                    {
                        playerTransform = parent;
                    }
                    
                    Debug.Log("Player Transform automatisch gefunden: " + playerTransform.name);
                }
                else
                {
                    Debug.LogWarning("Konnte kein passendes XR Rig finden. Bitte manuell zuweisen.");
                }
            }
        }
        
        // Erstelle einen AttachPoint als Kind der Plattform
        attachPoint = new GameObject("PlayerAttachPoint");
        attachPoint.transform.parent = transform;
        attachPoint.transform.localPosition = new Vector3(0, 0.1f, 0); // Leicht über der Plattform
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prüfe, ob der Spieler auf die Plattform teleportiert ist
        CheckForPlayer(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Alternative Erkennung über Collision
        CheckForPlayer(collision.gameObject);
    }

    private void CheckForPlayer(GameObject obj)
    {
        if (playerTransform == null) return;
        
        // Prüfe, ob es sich um das Player-Objekt oder ein relevantes Kind davon handelt
        bool isPlayerObject = (obj.transform == playerTransform);
        
        // Oder prüfe, ob es sich um ein relevantes Kind-Objekt handelt (z.B. Collider am Character Controller)
        if (!isPlayerObject && playerTransform != null)
        {
            // Prüfe ob es die Kamera ist
            if (obj.GetComponent<Camera>() != null && obj.transform.IsChildOf(playerTransform))
            {
                isPlayerObject = true;
            }
            
            // Prüfe auf XR-Controller
            if (!isPlayerObject && obj.name.Contains("Controller") && obj.transform.IsChildOf(playerTransform))
            {
                isPlayerObject = true;
            }
            
            // Prüfe auf CharacterController
            if (!isPlayerObject && obj.GetComponent<CharacterController>() != null && 
                (obj.transform == playerTransform || obj.transform.IsChildOf(playerTransform)))
            {
                isPlayerObject = true;
            }
        }
        
        if (isPlayerObject && !isPlayerAttached)
        {
            AttachPlayer();
        }
    }

    private void AttachPlayer()
    {
        if (playerTransform != null && !isPlayerAttached)
        {
            Debug.Log("Spieler an Plattform anheften");
            
            // Speichere den ursprünglichen Parent
            playerOriginalParent = playerTransform.parent;
            
            // Berechne den Offset, um den Spieler an der gleichen Position zu halten
            Vector3 originalWorldPos = playerTransform.position;
            
            // Setze den Spieler als Kind des AttachPoints
            playerTransform.parent = attachPoint.transform;
            
            // Behalt die Weltposition bei
            playerTransform.position = originalWorldPos;
            
            isPlayerAttached = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Prüfe, ob der Spieler die Plattform verlässt
        CheckForPlayerExit(other.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Alternative Erkennung über Collision
        CheckForPlayerExit(collision.gameObject);
    }

    private void CheckForPlayerExit(GameObject obj)
    {
        if (playerTransform == null) return;
        
        // Ähnliche Prüfung wie beim Betreten
        bool isPlayerObject = (obj.transform == playerTransform);
        
        if (!isPlayerObject && playerTransform != null)
        {
            if (obj.GetComponent<Camera>() != null && obj.transform.IsChildOf(playerTransform))
            {
                isPlayerObject = true;
            }
            
            if (!isPlayerObject && obj.name.Contains("Controller") && obj.transform.IsChildOf(playerTransform))
            {
                isPlayerObject = true;
            }
            
            if (!isPlayerObject && obj.GetComponent<CharacterController>() != null && 
                (obj.transform == playerTransform || obj.transform.IsChildOf(playerTransform)))
            {
                isPlayerObject = true;
            }
        }
        
        if (isPlayerObject && isPlayerAttached)
        {
            DetachPlayer();
        }
    }

    private void DetachPlayer()
    {
        if (playerTransform != null && isPlayerAttached)
        {
            Debug.Log("Spieler von Plattform lösen");
            
            // Speichere die Weltposition
            Vector3 originalWorldPos = playerTransform.position;
            
            // Setze den ursprünglichen Parent zurück
            playerTransform.parent = playerOriginalParent;
            
            // Behalt die Weltposition bei
            playerTransform.position = originalWorldPos;
            
            isPlayerAttached = false;
        }
    }
}