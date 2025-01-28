using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.InputSystem; // For the new Input System

public class ScreenshotManager : MonoBehaviour
{
    private AuthManager authManager;

    [Header("Supabase Configuration")]
    public string supabaseUrl;
    public string supabaseKey;
    public string bucketName = "public";

    private Keyboard keyboard;

    private void Start()
    {
        // Find the AuthManager component in the scene
        authManager = FindObjectOfType<AuthManager>();

        if (authManager == null)
        {
            Debug.LogError("AuthManager not found in the scene. Please add an AuthManager component.");
            return;
        }

        // Initialize the keyboard reference for the new Input System
        keyboard = Keyboard.current;
        if (keyboard == null)
        {
            Debug.LogError("No keyboard found. Ensure the Input System package is set up correctly.");
        }
    }

    private void Update()
    {
        // Block screenshot input entirely if the user is not logged in
        if (authManager.auth.CurrentUser == null)
        {
            return; // Exit Update() early if the player is not logged in
        }

        // Check for screenshot input only when the user is logged in
        if (keyboard != null && keyboard.bKey.wasPressedThisFrame)
        {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot()
    {
        // Ensure the user is logged in before proceeding
        if (authManager.auth.CurrentUser != null)
        {
            StartCoroutine(CaptureAndUploadScreenshot());
        }
        else
        {
            Debug.LogError("User is not logged in. Cannot take a screenshot.");
        }
    }

    private IEnumerator CaptureAndUploadScreenshot()
    {
        if (authManager.auth.CurrentUser == null)
        {
            Debug.LogError("User is not logged in. Cannot upload a screenshot.");
            yield break;
        }

        // Capture the screenshot
        string screenshotName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.jpeg";
        string filePath = Path.Combine(Application.persistentDataPath, screenshotName);

        int width = Screen.width;
        int height = Screen.height;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
        RenderTexture.active = renderTexture;

        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] imageData = screenshot.EncodeToJPG(100); // Encode as JPEG
        File.WriteAllBytes(filePath, imageData);
        Debug.Log($"Screenshot saved: {filePath}");

        Destroy(screenshot);

        yield return new WaitForSeconds(1f);

        if (File.Exists(filePath))
        {
            string userName = authManager.auth.CurrentUser.DisplayName ?? authManager.auth.CurrentUser.UserId;
            string folderPath = $"{bucketName}/{userName}";
            string uploadUrl = $"{supabaseUrl}/storage/v1/object/{folderPath}/{screenshotName}";

            UnityWebRequest request = new UnityWebRequest(uploadUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(imageData); // Use the byte array directly
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {supabaseKey}");
            request.SetRequestHeader("Content-Type", "image/jpeg"); // Correct MIME type for JPEG

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string publicUrl = $"{supabaseUrl}/storage/v1/object/public/{folderPath}/{screenshotName}";
                Debug.Log($"Screenshot uploaded! Public URL: {publicUrl}");

                // Save URL to Firebase
                SaveScreenshotUrlToFirebase(publicUrl);
            }
            else
            {
                Debug.LogError($"Failed to upload the screenshot to Supabase. Error: {request.error}");
            }

            File.Delete(filePath);
        }
        else
        {
            Debug.LogError("Screenshot file does not exist.");
        }
    }


    private void SaveScreenshotUrlToFirebase(string imageUrl)
    {
        if (authManager.auth.CurrentUser == null)
        {
            Debug.LogError("User is not logged in. Cannot save screenshot URL to Firebase.");
            return;
        }

        string userId = authManager.auth.CurrentUser.UserId;
        string username = authManager.auth.CurrentUser.DisplayName ?? "UnknownUser"; // Retrieve the username (or a default if null)

        // Reference to the "Screenshots" node with the username as a header
        DatabaseReference screenshotRef = authManager.dbReference
            .Child("Screenshots")
            .Child(username)
            .Child("images");

        screenshotRef.Push().SetValueAsync(imageUrl).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Screenshot URL successfully saved to Firebase Database under the username.");
            }
            else
            {
                Debug.LogError($"Failed to save screenshot URL to Firebase: {task.Exception}");
            }
        });
    }

    private void Awake()
    {
        // Ensure only one instance of this object exists
        if (FindObjectsOfType<ScreenshotManager>().Length > 1)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
    }


}
