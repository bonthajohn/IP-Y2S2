using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // For the new Input System

public class Inventory : MonoBehaviour
{
    public GameObject uiPrefab;  // Assign your UI Canvas prefab here
    private GameObject spawnedUI;

    public Transform playerCamera;  // Assign the player's camera (usually the VR headset)
    public float spawnDistance = 1.5f;  // Distance in front of the player

    [Header("VR Input Actions")]
    public InputActionProperty bButtonAction; // Assigned in the Inspector (Quest 2 B Button)

    void Start()
    {
        if (uiPrefab != null)
        {
            spawnedUI = Instantiate(uiPrefab);
            spawnedUI.SetActive(false); // Initially hide the UI
        }
    }

    private void OnEnable()
    {
        // Enable and assign the B button action
        bButtonAction.action.performed += OnBButtonPressed;
        bButtonAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the B button action when the object is disabled
        bButtonAction.action.performed -= OnBButtonPressed;
        bButtonAction.action.Disable();
    }

    void Update()
    {
        // For testing with XR Simulator (press "2" key using the new Input System)
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("2 key pressed (XR Simulator)");
            ToggleUI();
        }

        // Add keyboard input for testing the B button (press 'B' key)
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Debug.Log("B key pressed (Keyboard)");
            ToggleUI();
        }
    }

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("B Button Pressed (VR Controller)");
        ToggleUI();
    }

    void ToggleUI()
    {
        if (spawnedUI == null) return;

        bool isActive = spawnedUI.activeSelf;
        spawnedUI.SetActive(!isActive);

        if (!isActive)
        {
            PositionUI();
        }
    }

    void PositionUI()
    {
        if (spawnedUI == null) return;

        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;
        spawnPosition.y = playerCamera.position.y;
        Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

        spawnedUI.transform.position = spawnPosition;
        spawnedUI.transform.rotation = spawnRotation;
    }
}
