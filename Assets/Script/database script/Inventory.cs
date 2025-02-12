using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public GameObject questionMark;
    public GameObject actualItem;
}

public class Inventory : MonoBehaviour
{
    public GameObject uiPrefab;
    private GameObject spawnedUI;

    private Transform playerCamera;
    public float spawnDistance = 1.5f;

    [Header("VR Input Actions")]
    public InputActionProperty bButtonAction;

    [Header("Inventory Items (Setup in Inspector)")]
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    private Dictionary<string, InventoryItem> inventoryDictionary = new Dictionary<string, InventoryItem>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayerCamera();

        if (uiPrefab != null)
        {
            spawnedUI = Instantiate(uiPrefab);
            spawnedUI.SetActive(false);
        }

        foreach (var item in inventoryItems)
        {
            if (item.questionMark != null && item.actualItem != null)
            {
                inventoryDictionary[item.itemName] = item;
                item.questionMark.SetActive(true);
                item.actualItem.SetActive(false);
            }
        }
    }

    public void CollectItem(string itemName)
    {
        if (inventoryDictionary.ContainsKey(itemName))
        {
            InventoryItem item = inventoryDictionary[itemName];

            Debug.Log($"Collected: {itemName}");

            if (item.questionMark != null)
            {
                item.questionMark.SetActive(false);
            }

            if (item.actualItem != null)
            {
                item.actualItem.SetActive(true);
            }

            UpdateInventoryUI(); // 🔹 UI gets updated dynamically when collecting items
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' not found in inventory!");
        }
    }

    private void UpdateInventoryUI()
    {
        Debug.Log("Updating Inventory UI...");

        if (spawnedUI != null)
        {
            spawnedUI.SetActive(false);
            spawnedUI.SetActive(true);   // 🔹 Forces UI to refresh
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerCamera();
    }

    private void FindPlayerCamera()
    {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (cameraObject != null)
        {
            playerCamera = cameraObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'MainCamera'. Make sure your player camera has the correct tag.");
        }
    }

    private void OnEnable()
    {
        bButtonAction.action.performed += OnBButtonPressed;
        bButtonAction.action.Enable();
    }

    private void OnDisable()
    {
        bButtonAction.action.performed -= OnBButtonPressed;
        bButtonAction.action.Disable();
    }

    private void Update()
    {
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            ToggleUI();
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        ToggleUI();
    }

    private void ToggleUI()
    {
        if (spawnedUI == null || playerCamera == null) return;

        bool isActive = spawnedUI.activeSelf;
        spawnedUI.SetActive(!isActive);

        if (!isActive)
        {
            PositionUI();
        }
    }

    private void PositionUI()
    {
        if (spawnedUI == null || playerCamera == null) return;

        Vector3 spawnPosition = playerCamera.position + playerCamera.forward * spawnDistance;
        spawnPosition.y = playerCamera.position.y;
        Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

        spawnedUI.transform.position = spawnPosition;
        spawnedUI.transform.rotation = spawnRotation;
    }
}
