using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    public string gameName; // Assign this in the Unity Inspector
    public InputField timeInputField; // Assign an input field to enter time manually (optional)

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnButtonPress()
    {
        if (timeInputField != null)
        {
            double timeTaken;
            if (double.TryParse(timeInputField.text, out timeTaken))
            {
                UpdateGameProgress(gameName, timeTaken);
            }
            else
            {
                Debug.LogError("Invalid time format!");
            }
        }
        else
        {
            Debug.LogError("Time Input Field not assigned.");
        }
    }

    public void UpdateGameProgress(string gameName, double timeTaken)
    {
        if (user == null)
        {
            Debug.LogError("No user is logged in.");
            return;
        }

        string userId = user.UserId;
        string progressPath = $"Progress/{user.DisplayName}/{gameName}";

        dbReference.Child(progressPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving progress: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Dictionary<string, object> updatedProgress = new Dictionary<string, object>();

            if (snapshot.Exists)
            {
                string status = snapshot.Child("status").Value.ToString();
                int playedTime = int.Parse(snapshot.Child("playedTime").Value.ToString());
                double bestTime = double.Parse(snapshot.Child("bestTime").Value.ToString());

                updatedProgress["status"] = "completed";
                updatedProgress["playedTime"] = playedTime + 1;
                updatedProgress["bestTime"] = Mathf.Min((float)bestTime, (float)timeTaken);
            }
            else
            {
                updatedProgress["status"] = "completed";
                updatedProgress["playedTime"] = 1;
                updatedProgress["bestTime"] = timeTaken;
            }

            dbReference.Child(progressPath).UpdateChildrenAsync(updatedProgress).ContinueWith(updateTask =>
            {
                if (updateTask.IsFaulted)
                {
                    Debug.LogError("Error updating progress: " + updateTask.Exception);
                }
                else
                {
                    Debug.Log("Game progress updated successfully.");
                }
            });
        });
    }
}
