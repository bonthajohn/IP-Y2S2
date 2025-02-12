using UnityEngine;
using UnityEngine.InputSystem;

public class Parts : MonoBehaviour
{
    public string itemName;
    public string mysteryBoxTag; // Tag for the question mark/mystery box
    public string actualItemTag; // Tag for the actual item that should appear

    private bool isCollected = false;
    public InputActionProperty yButtonAction;

    private bool isPlayerNearby = false;
    private Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory system not found in the scene!");
        }
    }

    private void OnEnable()
    {
        yButtonAction.action.performed += OnYButtonPressed;
        yButtonAction.action.Enable();
    }

    private void OnDisable()
    {
        yButtonAction.action.performed -= OnYButtonPressed;
        yButtonAction.action.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log($"Player entered pickup range for {itemName}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log($"Player left pickup range for {itemName}");
        }
    }

    private void Update()
    {
        if (isPlayerNearby && !isCollected && Keyboard.current.fKey.wasPressedThisFrame)
        {
            CollectItem();
        }
    }

    private void OnYButtonPressed(InputAction.CallbackContext context)
    {
        if (isPlayerNearby && !isCollected)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (inventory != null)
        {
            Debug.Log($"Picking up {itemName}");

            // Hide Mystery Box (question mark)
            GameObject mysteryBox = GameObject.FindGameObjectWithTag(mysteryBoxTag);
            if (mysteryBox != null)
            {
                Debug.Log($"{itemName} - Hiding Mystery Box");
                mysteryBox.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Mystery Box with tag '{mysteryBoxTag}' not found!");
            }

            // Show Actual Key
            GameObject actualItem = GameObject.FindGameObjectWithTag(actualItemTag);
            if (actualItem != null)
            {
                Debug.Log($"{itemName} - Showing Actual Key");
                actualItem.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Actual Item with tag '{actualItemTag}' not found!");
            }

            isCollected = true;
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Inventory reference is missing!");
        }
    }
}
