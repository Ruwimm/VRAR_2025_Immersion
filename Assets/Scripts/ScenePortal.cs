using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ScenePortal : MonoBehaviour
{
    public string sceneName = "ZielSzene";
    public InputActionReference activateInput; // z.B. Trigger-Taste vom Controller
    private bool isPlayerNear = false;

    private void OnEnable()
    {
        activateInput.action.Enable();
    }

    private void OnDisable()
    {
        activateInput.action.Disable();
    }

    void Update()
    {   
        Debug.Log("isPlayerNear: " + isPlayerNear);
        Debug.Log("activateInput.action.WasPressedThisFrame(): " + activateInput.action.WasPressedThisFrame());
        if (isPlayerNear && activateInput.action.WasPressedThisFrame())
        {
            Debug.Log("Teleporting to " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter: " + other.name);
        if (other.CompareTag("MainCamera")) // XR Camera Tag setzen!
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isPlayerNear = false;
        }
    }
}
