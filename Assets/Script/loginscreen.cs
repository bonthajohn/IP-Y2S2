using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    public Camera playerCamera; // Reference to the player's camera
    public Vector3 offset; // Offset distance from the camera

    private RectTransform canvasRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // Get the RectTransform component of the Canvas
        canvasRectTransform = GetComponent<RectTransform>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // Automatically use the main camera if none is assigned
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the position of the Canvas to follow the camera
        if (canvasRectTransform != null && playerCamera != null)
        {
            // Position the Canvas at a fixed distance from the camera
            canvasRectTransform.position = playerCamera.transform.position + playerCamera.transform.forward * offset.z + playerCamera.transform.up * offset.y + playerCamera.transform.right * offset.x;

            // Optional: Make the Canvas always face the camera
            canvasRectTransform.LookAt(playerCamera.transform);
        }
    }
}
