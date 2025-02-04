using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;  // For the new Input System

public class Inventory : MonoBehaviour
{
    public GameObject uiPrefab;  // Assign your UI Canvas prefab here
    private GameObject spawnedUI;

    public Transform playerCamera;  // Assign the player's camera (usually the VR headset)
    public float spawnDistance = 1.5f;  // Distance in front of the player

    void Start()
    {
        if (uiPrefab != null)
        {
            spawnedUI = Instantiate(uiPrefab);
            spawnedUI.SetActive(false); // Initially hide the UI
        }
    }

    void Update()
    {
        // Check if the B button is pressed (for VR Quest 2 controller)
        //if (OVRInput.GetDown(OVRInput.Button.Two))
        //{
        //    Debug.Log("B button pressed (VR Controller)");  // Debug log to check if button press is detected
        //    ToggleUI();
        //}

        // For testing with XR Simulator (press "2" key using the new Input System)
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("2 key pressed (XR Simulator)");
            ToggleUI();
        }

        // Add keyboard input for testing the B button (press 'B' key)
        if (Keyboard.current.bKey.wasPressedThisFrame)  // Simulate pressing the "B" key
        {
            Debug.Log("B key pressed (Keyboard)");  // Debug log for the "B" key press
            ToggleUI();
        }
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