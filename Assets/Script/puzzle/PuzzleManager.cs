using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import the TextMesh Pro namespace
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public PuzzleSlot[] puzzleSlots; // Array of slots, each linking to a puzzle piece

    private float timer = 0f; // Timer variable to track the time elapsed
    private bool isPuzzleComplete = false; // To check if the puzzle is completed
    public TextMeshProUGUI timerText; // TextMesh Pro UI element to display the timer (optional)

    private AuthManager authManager; // Reference to the AuthManager
    private DatabaseReference databaseReference; // Reference to the Firebase Realtime Database

    private string username; // Store the player's username

    private void Start()
    {
        // Find the AuthManager component in the scene
        authManager = FindObjectOfType<AuthManager>();

        if (authManager == null)
        {
            Debug.LogError("AuthManager not found in the scene. Please add an AuthManager component.");
            return;
        }

        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Assuming username is stored in AuthManager
            username = authManager.auth.CurrentUser.DisplayName ?? authManager.auth.CurrentUser.UserId;
        });

        // Initialize puzzle slots
        foreach (var slot in puzzleSlots)
        {
            if (slot.slotObject == null)
            {
                Debug.LogError("A slot is missing its assigned GameObject!");
            }
        }
    }

    private void Update()
    {
        if (!isPuzzleComplete)
        {
            timer += Time.deltaTime; // Increment timer every frame
            if (timerText != null)
            {
                DisplayTime(timer); // Display the formatted timer
            }
        }
    }

    public void CheckPuzzleCompletion()
    {
        foreach (var slot in puzzleSlots)
        {
            if (!slot.IsPieceCorrect())
            {
                Debug.Log("Puzzle is not yet complete!");
                return;
            }
        }
        Debug.Log("Puzzle is complete!");
        isPuzzleComplete = true;

        // Update Firebase after the puzzle is completed
        if (databaseReference != null && !string.IsNullOrEmpty(username))
        {
            UpdatePeppermintPuzzleData(timer);
        }
    }

    private void UpdatePeppermintPuzzleData(float finalTime)
    {
        string path = $"Progress/{username}/peppermintPuzzle";

        // Fetch the current data for the user
        DatabaseReference userRef = databaseReference.Child(path);

        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch data for peppermintPuzzle");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int currentBestTime = int.Parse(snapshot.Child("bestTime").Value.ToString());
                int currentPlayedTime = int.Parse(snapshot.Child("playedTime").Value.ToString());
                string currentStatus = snapshot.Child("status").Value.ToString();

                // Calculate best time
                int newBestTime = currentBestTime == 0 || finalTime < currentBestTime ? Mathf.FloorToInt(finalTime) : currentBestTime;

                // Increment played time
                int newPlayedTime = currentPlayedTime + 1;

                // Update status if completed
                string newStatus = currentStatus == "not_played" && finalTime > 0 ? "completed" : currentStatus;

                // Write the updated values to Firebase
                userRef.Child("bestTime").SetValueAsync(newBestTime);
                userRef.Child("playedTime").SetValueAsync(newPlayedTime);
                userRef.Child("status").SetValueAsync(newStatus);
            }
        });
    }

    public void ResetPuzzle()
    {
        foreach (var slot in puzzleSlots)
        {
            slot.ResetSlot();
        }

        timer = 0f; // Reset timer
        isPuzzleComplete = false; // Reset puzzle completion status
        if (timerText != null)
        {
            timerText.text = "Time: 00:00"; // Reset the UI display (optional)
        }
    }

    // Helper method to display the time in MM:SS format
    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60); // Get the minutes
        int seconds = Mathf.FloorToInt(time % 60); // Get the remaining seconds

        // Update the timerText to display minutes and seconds, ensuring two digits for seconds
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}

[System.Serializable]
public class PuzzleSlot
{
    public GameObject slotObject; // The actual GameObject representing the slot
    public PuzzlePiece puzzlePiece; // The puzzle piece that should go in this slot

    public bool IsPieceCorrect()
    {
        if (puzzlePiece == null || slotObject == null) return false;
        return Vector3.Distance(puzzlePiece.transform.position, slotObject.transform.position) < 0.1f;
    }

    public void ResetSlot()
    {
        if (puzzlePiece != null)
        {
            puzzlePiece.ResetPiece();
        }
    }
}
